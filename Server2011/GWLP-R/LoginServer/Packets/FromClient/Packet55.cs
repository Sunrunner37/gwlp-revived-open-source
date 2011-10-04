using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 55)]
        public class Packet55 : IPacket
        {
                public class PacketSt55 : IPacketTemplate
                {
                        public UInt16 Header { get { return 55; } }
                        public UInt32 Data1;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Data2;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Data3;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt55>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt55> pParser;
        }
}
