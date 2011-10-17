using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 309)]
        public class P309_ItemPagePacket : IPacket
        {
                public class PacketSt309 : IPacketTemplate
                {
                        public UInt16 Header { get { return 309; } }
                        public UInt16 ItemStreamID;
                        public byte StorageType; //Bags=0x1,Equiped=0x2,NotCollected=0x3 ???,Storage=0x4,StorageMaterial=0x5
                        public byte StorageID; //see GameServer.Enums.ItemStorage
                        public UInt16 PageID;
                        public byte Slots;
                        public UInt32 ItemLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt309>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt309)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt309> pParser;

        }
}
