using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 308)]
        public class P308_ItemLocation : IPacket
        {
                public class PacketSt308 : IPacketTemplate
                {
                        public UInt16 Header { get { return 308; } }
                        public UInt16 ItemStreamID;
                        public UInt32 ItemLocalID;
                        public UInt16 PageID;
                        public byte UserSlot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt308>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt308)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt308> pParser;

        }
}
