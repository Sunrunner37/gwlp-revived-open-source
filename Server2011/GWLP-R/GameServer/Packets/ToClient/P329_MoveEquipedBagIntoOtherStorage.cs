using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 329)]
        public class P329_MoveEquipedBagIntoOtherStorage : IPacket
        {
                public class PacketSt329 : IPacketTemplate
                {
                        public UInt16 Header { get { return 329; } }
                        public UInt16 ItemStreamID;
                        public UInt32 BagLocalID;
                        public UInt16 NewPageID;
                        public byte NewSlot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt329>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt329)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt329> pParser;

        }
}
