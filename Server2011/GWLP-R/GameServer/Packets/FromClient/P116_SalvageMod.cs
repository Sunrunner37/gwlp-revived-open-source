using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 116)]
        public class P116_SalvageMod : IPacket
        {
                public class PacketSt116 : IPacketTemplate
                {
                        public UInt16 Header { get { return 116; } }
                        public byte ModIndex;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt116>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt116> pParser;
        }
}
