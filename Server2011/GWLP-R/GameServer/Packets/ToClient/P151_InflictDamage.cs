using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 151)]
        public class P151_InflictDamage : IPacket
        {
                public class PacketSt151 : IPacketTemplate
                {
                        public UInt16 Header { get { return 151; } }
                        public UInt32 DamageClass; // ??? (melee 0x10, sacrifice 0x37 etc)
                        public UInt32 TargetAgentID;
                        public UInt32 DamageDealerAgentID;
                        public Single Damage; // float representation of health loss in percent of max health -0.08 = 8% hp loss
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt151>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt151)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt151> pParser;

        }
}
