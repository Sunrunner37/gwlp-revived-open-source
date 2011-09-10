using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 343)]
        public class P343_ItemGeneral : IPacket
        {
                public class PacketSt343 : IPacketTemplate
                {
                        public UInt16 Header { get { return 343; } }
                        public UInt32 ItemLocalID;
                        public UInt32 ItemFileID; // 3d model file
                        public byte Type;
                        public byte Data2; // color?
                        public UInt16 Data3;
                        public UInt16 Data4;
                        public byte Data5;
                        public UInt32 ItemFlags; // bitfield containig rarity, uniqueness, is unidentified, etc.
                        public UInt32 MerchantPrice;
                        public UInt32 ItemID;
                        public UInt32 Quantity;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public string ItemNameHash;
                        public byte ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 256)]
                        public UInt32[] ItemMods; // guessed: 1st byte: mod flags; 2nd byte mod value; next 2 bytes ModID
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt343>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt343)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt343> pParser;

        }
}
