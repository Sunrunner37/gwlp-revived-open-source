using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 321)]
        public class P321_MoveItem : IPacket
        {
                public class PacketSt321 : IPacketTemplate
                {
                        public UInt16 Header { get { return 321; } }
                        public UInt16 ItemStreamID;
                        public UInt32 ItemLocalID;
                        public UInt16 NewPageID;
                        public byte NewSlot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt321>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt321)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt321> pParser;

        }
}
