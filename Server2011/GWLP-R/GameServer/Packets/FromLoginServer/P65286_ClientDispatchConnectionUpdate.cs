using System;
using System.Linq;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65286)]
        public class P65286_ClientDispatchConnectionUpdate : IPacket
        {
                public class PacketSt65286 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65286; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] ConnectionInfo;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                        public UInt32 OldMapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65286>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65286();
                        pParser(pack, message.PacketData);

                        // get the client/character
                        var client = GameServerWorld.Instance.Get<DataClient>(new AccID(pack.AccID));
                        var chara = client.Character;

                        if (client.Data.Status != SyncStatus.Dispatching)
                        {
                                // the client will be transfered to another server

                                // set the client/character status
                                client.Data.Status = SyncStatus.Dispatching;
                                if (chara != null) chara.Data.Player = PlayStatus.LoadingInstance;

                                // set the client/character mapID
                                client.Data.MapID = new MapID(pack.MapID);
                                if (chara != null) chara.Data.MapID = new MapID(pack.MapID);
                        }

                        // get the GameMapID
                        var gameMapID = 1;
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var cmapID = (int) pack.MapID;
                                var maps = from m in db.mapsMasterData
                                           where m.mapID == cmapID
                                           select m;

                                if (maps.Count() > 0)
                                {
                                        gameMapID = maps.First().gameMapID;
                                }
                        }

                        // Note: DISPATCH
                        var chatMsg = new NetworkMessage(client.Data.NetID)
                        {
                                PacketTemplate = new P406_Dispatch.PacketSt406()
                                {
                                        ConnectionInfo = pack.ConnectionInfo,
                                        Key1 = pack.Key1,
                                        Key2 = pack.Key2,
                                        ZoneID = (ushort)gameMapID,
                                        Region = 0,
                                        IsOutpost = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                        // send dispatch connection close
                        // Note: DISPATCH CONNECTION TERMINATION
                        var ilChar = new NetworkMessage(client.Data.NetID)
                        {
                                PacketTemplate = new P141_DispatchConnectionTermination.PacketSt141()
                                {
                                        GameMapID = (ushort)client.Data.MapID.Value,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ilChar);

                        // this last heartbeat has static time data because we can! :P
                        // Note: HEARTBEAT
                        var heartBeat = new NetworkMessage(client.Data.NetID)
                        {
                                PacketTemplate = new P019_Heartbeat.PacketSt19()
                                {
                                        Data1 = 250
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(heartBeat);

                        // remove the char from the current map
                        var map = GameServerWorld.Instance.Get<DataMap>(new MapID(pack.OldMapID));
                        if (map != null)
                        {
                                // get the char and remove it
                                map.Remove(map.Get<DataCharacter>(new CharID(pack.CharID)));
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65286> pParser;
        }
}
