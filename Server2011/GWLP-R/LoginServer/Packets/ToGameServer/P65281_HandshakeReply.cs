using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToGameServer
{
        [PacketAttributes(IsIncoming = false, Header = 65281)]
        public class P65281_HandshakeReply : IPacket
        {
                public class PacketSt65281 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65281; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65281>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65281)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65281> pParser;

        }
}
