using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 19)]
        public class P19_CommandAll : IPacket
        {
                public class PacketSt19 : IPacketTemplate
                {
                        public UInt16 Header { get { return 19; } }
                        public Single X;
                        public Single Y;
                        public UInt32 Plane;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt19>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt19> pParser;
        }
}
