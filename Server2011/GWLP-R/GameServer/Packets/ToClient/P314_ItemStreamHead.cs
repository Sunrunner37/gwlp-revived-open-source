using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 314)]
        public class P314_ItemStreamHead : IPacket
        {
                public class PacketSt314 : IPacketTemplate
                {
                        public UInt16 Header { get { return 314; } }
                        public UInt16 ItemStreamID;
                        public byte Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt314>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt314)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt314> pParser;

        }
}
