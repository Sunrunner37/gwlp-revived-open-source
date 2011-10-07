using System;
using System.Linq;
using GameServer.DataBase;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65283)]
        public class P65283_BuildMapRequest : IPacket
        {
                public class PacketSt65283 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65283; } }
                        public UInt32 MapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65283>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65283();
                        pParser(pack, message.PacketData);

                        // build the map
                        GameServerWorld.Instance.BuildMap(new MapID(pack.MapID), pack.IsOutpost == 0 ? false : true, pack.IsPvE == 0 ? false : true);

                        // get availabe maps: (as ushort array of mapID's)
                        var ids = GameServerWorld.Instance.GetMapIDs().Select(x => (ushort)x.Value).ToArray();

                        // create reply
                        // Note: SERVER STATS
                        var reply = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P65282_ServerStatsReply.PacketSt65282
                                {
                                        ArraySize1 = (ushort)ids.Length,
                                        MapIDs = ids,
                                        Utilization = (byte)NetworkManager.Instance.GetUtilization(),
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(reply);
                               
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65283> pParser;
        }
}
