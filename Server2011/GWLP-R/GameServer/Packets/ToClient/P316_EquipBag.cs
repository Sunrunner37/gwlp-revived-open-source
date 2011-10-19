using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 316)]
        public class P316_EquipBag : IPacket
        {
                public class PacketSt316 : IPacketTemplate
                {
                        public UInt16 Header { get { return 316; } }
                        public UInt16 ItemStreamID;
                        public byte StorageID; // enum ItemStorage
                        public UInt16 PageID; // new page id for the bag
                        public byte Slots;
                        public UInt32 BagLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt316>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt316)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt316> pParser;

        }
}
