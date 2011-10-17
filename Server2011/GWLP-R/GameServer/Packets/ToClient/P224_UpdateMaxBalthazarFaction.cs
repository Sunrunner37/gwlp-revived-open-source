using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 224)]
        public class P224_UpdateMaxBalthazarFaction : IPacket
        {
                public class PacketSt224 : IPacketTemplate
                {
                        public UInt16 Header { get { return 224; } }
                        public UInt32 MaxBalthazarFaction;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt224>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt224)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt224> pParser;

        }
}
