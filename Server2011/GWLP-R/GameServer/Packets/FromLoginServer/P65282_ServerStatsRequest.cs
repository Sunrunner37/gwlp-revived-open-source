using System;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65282)]
        public class P65282_ServerStatsRequest : IPacket
        {
                public class PacketSt65282 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65282; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65282>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // nothing to parse here ;)

                        // create reply
                        var reply = new NetworkMessage(message.NetID) { PacketTemplate = new P65282_ServerStatsRequest.PacketSt65282() };
                        var ids = World.GetMapIDs();
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).ArraySize1 = (UInt16)ids.Length;
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).MapIDs = ids;
                        ((P65282_ServerStatsReply.PacketSt65282)reply.PacketTemplate).Utilization = (byte)NetworkManager.Instance.GetUtilization();
                        QueuingService.PostProcessingQueue.Enqueue(reply);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65282> pParser;
        }
}
