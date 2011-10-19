using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 56)]
        public class P056_RemoveSkillEffect : IPacket
        {
                public class PacketSt56 : IPacketTemplate
                {
                        public UInt16 Header { get { return 56; } }
                        public UInt32 AgentID;
                        public UInt32 SkillEffectID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt56>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt56)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt56> pParser;

        }
}
