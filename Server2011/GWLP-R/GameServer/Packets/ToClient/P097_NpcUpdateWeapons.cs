using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 97)]
        public class P097_NpcUpdateWeapons : IPacket
        {
                public class PacketSt97 : IPacketTemplate
                {
                        public UInt16 Header { get { return 97; } }
                        public UInt32 AgentID;
                        public UInt32 LeadhandItemLocalID;
                        public UInt32 OffhandItemLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt97>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt97)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt97> pParser;

        }
}
