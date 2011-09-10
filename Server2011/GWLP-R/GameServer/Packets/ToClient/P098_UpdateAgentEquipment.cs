using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 98)]
        public class P098_UpdateAgentEquipment : IPacket
        {
                public class PacketSt98 : IPacketTemplate
                {
                        public UInt16 Header { get { return 98; } }
                        public UInt32 ID1;
                        public UInt32 Weapon1;
                        public UInt32 Weapon2;
                        public UInt32 Chest;
                        public UInt32 Feet;
                        public UInt32 Legs;
                        public UInt32 Arms;
                        public UInt32 Head;
                        public UInt32 Data8;
                        public UInt32 Data9;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt98>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt98)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt98> pParser;

        }
}
