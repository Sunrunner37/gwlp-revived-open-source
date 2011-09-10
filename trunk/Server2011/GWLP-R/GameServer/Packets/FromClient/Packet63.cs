using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 63)]
        public class Packet63 : IPacket
        {
                public class PacketSt63 : IPacketTemplate
                {
                        public UInt16 Header { get { return 63; } }
                        public UInt32 Data1;
                        public UInt32 Data2;
                        public UInt32 ID1;
                        public byte Data3;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt63>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt63> pParser;
        }
}
