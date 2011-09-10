using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 55)]
        public class P055_ClickLocation : IPacket
        {
                public class PacketSt55 : IPacketTemplate
                {
                        public UInt16 Header { get { return 55; } }
                        public Single Data1;
                        public Single Data2;
                        public UInt32 Data3;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt55>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt55();
                        pParser((PacketSt55)message.PacketTemplate, message.PacketData);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt55> pParser;
        }
}
