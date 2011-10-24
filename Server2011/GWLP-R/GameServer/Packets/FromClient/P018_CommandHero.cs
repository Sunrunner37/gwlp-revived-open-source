using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 18)]
        public class P018_CommandHero : IPacket
        {
                public class PacketSt18 : IPacketTemplate
                {
                        public UInt16 Header { get { return 18; } }
                        public UInt32 HeroID;
                        public Single X;
                        public Single Y;
                        public UInt32 Plane;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt18>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt18> pParser;
        }
}
