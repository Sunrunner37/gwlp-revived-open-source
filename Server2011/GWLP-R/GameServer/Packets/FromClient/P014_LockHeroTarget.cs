using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 14)]
        public class P014_LockHeroTarget : IPacket
        {
                public class PacketSt14 : IPacketTemplate
                {
                        public UInt16 Header { get { return 14; } }
                        public UInt32 HeroID;//AgentID
                        public UInt32 TargetID;//AgentID
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt14>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt14> pParser;
        }
}
