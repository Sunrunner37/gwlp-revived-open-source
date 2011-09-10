using System;
using System.Diagnostics;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 35)]
        public class P35_ClientID : IPacket
        {
                public class PacketSt35 : IPacketTemplate
                {
                        public UInt16 Header { get { return 35; } }
                        public UInt32 Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt35>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt35();
                        pParser((PacketSt35)message.PacketTemplate, message.PacketData);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt35> pParser;
        }
}
