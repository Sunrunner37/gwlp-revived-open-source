using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 48)]
        public class P048_UpdateAttributeLevel : IPacket
        {
                public class PacketSt48 : IPacketTemplate
                {
                        public UInt16 Header { get { return 48; } }
                        public UInt32 AgentID;
                        public byte AttributeID; // enum Attribute
                        public byte NormalLevel;
                        public byte CurrentLevel;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt48>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt48)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt48> pParser;

        }
}
