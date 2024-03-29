using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 147)]
        public class P147_UpdateGenericValueInt : IPacket
        {
                public class PacketSt147 : IPacketTemplate
                {
                        public UInt16 Header { get { return 147; } }
                        public UInt32 ValueID;
                        public UInt32 AgentID;
                        public UInt32 Value;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt147>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt147)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt147> pParser;

        }
}
