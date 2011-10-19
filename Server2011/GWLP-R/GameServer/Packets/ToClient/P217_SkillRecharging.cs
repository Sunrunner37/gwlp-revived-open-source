using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 217)]
        public class P217_SkillRecharging : IPacket
        {
                public class PacketSt217 : IPacketTemplate
                {
                        public UInt16 Header { get { return 217; } }
                        public UInt32 CasterAgentID;
                        public UInt16 SkillID;
                        public UInt32 Data2;
                        public UInt32 Recharge;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt217>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt217)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt217> pParser;

        }
}
