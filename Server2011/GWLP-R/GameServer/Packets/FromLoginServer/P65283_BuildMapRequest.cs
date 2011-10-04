using System;
using System.Linq;
using GameServer.DataBase;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
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
                        message.PacketTemplate = new PacketSt65283();
                        pParser((PacketSt65283)message.PacketTemplate, message.PacketData);

                        World.BuildMap((int)((PacketSt65283)message.PacketTemplate).MapID);

                        // response
                        var reply = new NetworkMessage(message.NetID) { PacketTemplate = new P65282_ServerStatsReply.PacketSt65282() };
                        var ids = World.GetMapIDs();
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).ArraySize1 = (UInt16)ids.Length;
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).MapIDs = ids;
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).Utilization = (byte)NetworkManager.Instance.GetUtilization();
                        QueuingService.PostProcessingQueue.Enqueue(reply);
                               
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65283> pParser;
        }
}
