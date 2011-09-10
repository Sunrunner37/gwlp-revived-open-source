using System;
using GameServer.Packets.ToClient;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 5)]
        public class P005_Ping : IPacket
        {
                public class PacketSt5 : IPacketTemplate
                {
                        public UInt16 Header { get { return 5; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt5>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        //throw new NotImplementedException();
                        var chatMsg = new NetworkMessage(message.NetID);
                        chatMsg.PacketTemplate = new P002_PingReply.PacketSt2()
                        {
                                Data1 = 45
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt5> pParser;
        }
}
