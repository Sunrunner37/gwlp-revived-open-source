using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 311)]
        public class P311_UpdateGoldInStorage : IPacket
        {
                public class PacketSt311 : IPacketTemplate
                {
                        public UInt16 Header { get { return 311; } }
                        public UInt16 ItemStreamID;
                        public UInt32 GoldInStorage;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt311>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt311)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt311> pParser;

        }
}
