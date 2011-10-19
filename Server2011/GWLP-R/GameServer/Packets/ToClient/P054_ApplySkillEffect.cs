using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 54)]
        public class P054_ApplySkillEffect : IPacket
        {
                public class PacketSt54 : IPacketTemplate
                {
                        public UInt16 Header { get { return 54; } }
                        public UInt32 AgentID;
                        public UInt16 SkillID;
                        public UInt32 Data2;
                        public UInt32 SkillEffectID;
                        public Single Duration;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt54>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt54)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt54> pParser;

        }
}
