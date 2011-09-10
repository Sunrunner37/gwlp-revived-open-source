using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 2)]
        public class P02_ComputerHash : IPacket
        {
                public class PacketSt2 : IPacketTemplate
                {
                        public UInt16 Header { get { return 2; } }
                        public UInt32 Data1;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt2>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt2();
                        pParser((PacketSt2)message.PacketTemplate, message.PacketData);

                        // do nothing here

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt2> pParser;
        }
}
