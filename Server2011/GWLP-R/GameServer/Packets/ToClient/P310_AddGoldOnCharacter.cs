using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 310)]
        public class P310_AddGoldOnCharacter : IPacket
        {
                public class PacketSt310 : IPacketTemplate
                {
                        public UInt16 Header { get { return 310; } }
                        public UInt16 ItemStreamID;
                        public UInt32 GoldOnCharacter;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt310>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt310)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt310> pParser;

        }
}
