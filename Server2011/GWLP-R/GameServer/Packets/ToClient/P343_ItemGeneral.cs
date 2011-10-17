using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
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
                        public UInt32 LocalID;
                        public UInt32 FileID; // 3d model file
                        public byte ItemType; //sword 1b, axe 2
                        public byte Data2; //sword 3, axe 6
                        public UInt16 DyeColor;
                        public UInt16 Data4; // Standard Color?
                        public byte CanBeDyed;
                        public UInt32 Flags;
                        public UInt32 MerchantPrice;
                        public UInt32 ItemID; //sword 46C2, axe 45D4   icon?
                        public UInt32 Quantity;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public string NameHash;
                        public byte NumStats;
                        [PacketFieldType(ConstSize = false, MaxSize = 256)]
                        public UInt32[] Stats;
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
