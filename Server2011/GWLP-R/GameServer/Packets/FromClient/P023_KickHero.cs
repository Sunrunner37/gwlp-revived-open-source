using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 23)]
        public class P023_KickHero : IPacket
        {
                public class PacketSt23 : IPacketTemplate
                {
                        public UInt16 Header { get { return 23; } }
                        public UInt16 HeroID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt23>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt23> pParser;
        }
}
