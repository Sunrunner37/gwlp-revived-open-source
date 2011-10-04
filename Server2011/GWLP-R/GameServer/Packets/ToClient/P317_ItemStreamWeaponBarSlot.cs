using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 317)]
        public class P317_ItemStreamWeaponBarSlot : IPacket
        {
                public class PacketSt317 : IPacketTemplate
                {
                        public UInt16 Header { get { return 317; } }
                        public UInt16 ItemStreamID;
                        public byte SlotNumber;
                        public UInt32 Data3;
                        public UInt32 Data4;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt317>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt317)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt317> pParser;

        }
}
