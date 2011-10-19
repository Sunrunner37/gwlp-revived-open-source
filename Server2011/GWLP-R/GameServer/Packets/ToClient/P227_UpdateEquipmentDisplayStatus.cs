using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 227)]
        public class P227_UpdateEquipmentDisplayStatus : IPacket
        {
                public class PacketSt227 : IPacketTemplate
                {
                        public UInt16 Header { get { return 227; } }
                        public UInt32 DisplayStatus; // enum EquipmentDisplayPart*EquipmentDisplayStatus
                        public UInt32 DisplayPart; // enum EquipmentDisplayPart*EquipmentDisplayStatus.ShowAlways
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt227>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt227)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt227> pParser;

        }
}
