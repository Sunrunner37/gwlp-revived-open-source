using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 74)]
        public class P074_NpcGeneralStats : IPacket
        {
                public class PacketSt74 : IPacketTemplate
                {
                        public UInt16 Header { get { return 74; } }
                        public UInt32 NpcID;
                        public UInt32 FileID;
                        public UInt32 Data1;
                        public UInt32 Scale;
                        public UInt32 Data2;
                        public UInt32 ProfessionFlags; // bitfield: 0-15 profession stuff, 16 show name 
                        public byte Profession;
                        public byte Level;
                        //[PacketFieldType(ConstSize = false, MaxSize = 8)]
                        //public string Appearance;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 8)]
                        public byte[] Appearance;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt74>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt74)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt74> pParser;

        }
}
