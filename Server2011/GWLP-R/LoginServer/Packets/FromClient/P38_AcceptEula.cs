using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 38)]
        public class P38_AcceptEula : IPacket
        {
                public class PacketSt38 : IPacketTemplate
                {
                        public UInt16 Header { get { return 38; } }
                        public byte EulaNumber; // was given by the server
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt38>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt38();
                        pParser((PacketSt38)message.PacketTemplate, message.PacketData);

                        // do nothing here

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt38> pParser;
        }
}
