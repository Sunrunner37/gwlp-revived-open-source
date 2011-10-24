using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 84)]
        public class P084_SetSkillbarSkill : IPacket
        {
                public class PacketSt84 : IPacketTemplate
                {
                        public UInt16 Header { get { return 84; } }
                        public UInt32 AgentID;
                        public UInt32 Slot;
                        public UInt32 SkillID;
                        public UInt32 Flag;//0
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt84>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt84> pParser;
        }
}
