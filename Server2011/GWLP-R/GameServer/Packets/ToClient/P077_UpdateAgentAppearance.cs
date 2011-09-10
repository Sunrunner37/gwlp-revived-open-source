using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 77)]
        public class P077_UpdateAgentAppearance : IPacket
        {
                public class PacketSt77 : IPacketTemplate
                {
                        public UInt16 Header { get { return 77; } }
                        public UInt32 Data1;
                        public UInt32 ID1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)] // changed, was UInt32
                        public byte[] Appearance;                        // ~
                        public byte Data2;
                        public UInt32 Data3;
                        public UInt32 Data4;
                        [PacketFieldType(ConstSize = false, MaxSize = 32)]
                        public string Name;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt77>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt77)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt77> pParser;

        }
}
