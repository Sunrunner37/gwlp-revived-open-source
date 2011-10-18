using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 148)]
        public class P148_SkillVisuals : IPacket
        {
                public class PacketSt148 : IPacketTemplate
                {
                        public UInt16 Header { get { return 148; } }
                        public UInt32 PacketUsage; // defines how the packet is used: 0x14 = [VisualID=EffectID on AffectedAgentID like the lightning surge after invoke] , 0x3C = [VisualID=SkillID of the Skill casted by AffectedAgentID applied on AffectedAgentID]
                        public UInt32 AffectedAgentID;
                        public UInt32 OtherInvolvedAgentID;
                        public UInt32 VisualID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt148>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt148)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt148> pParser;

        }
}
