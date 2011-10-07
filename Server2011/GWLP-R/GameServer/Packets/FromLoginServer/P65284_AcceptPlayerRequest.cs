using System;
using GameServer.Enums;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65284)]
        public class P65284_AcceptPlayerRequest : IPacket
        {
                public class PacketSt65284 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65284; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65284>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65284();
                        pParser(pack, message.PacketData);

                        // create a new client
                        var newClientData = new ClientData
                        {
                                AccID = new AccID(pack.AccID),
                                CharID = new CharID(pack.CharID),
                                MapID = new MapID(pack.MapID),
                                SecurityKeys = new[]{pack.Key1, pack.Key2},
                                Status = SyncStatus.Unauthorized,
                        };             

                        // try to add the client
                        var added = GameServerWorld.Instance.Add(new DataClient(newClientData));

                        // response
                        var reply = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P65284_AcceptPlayerReply.PacketSt65284
                                {
                                        AccID = newClientData.AccID.Value,
                                        Success = (byte)(added? 1 : 0),
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(reply);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65284> pParser;
        }
}
