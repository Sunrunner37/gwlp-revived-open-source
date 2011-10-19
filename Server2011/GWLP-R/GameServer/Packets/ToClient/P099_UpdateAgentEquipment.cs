using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 99)]
        public class P099_UpdateAgentEquipment : IPacket
        {
                public class PacketSt99 : IPacketTemplate
                {
                        public UInt16 Header { get { return 99; } }
                        public UInt32 AgentID;
                        public UInt32 EquipmentSlot; // enum AgentEquipment
                        public UInt32 ItemLocalID; // item to equip, 0 removes item
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt99>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt99)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt99> pParser;

        }
}
