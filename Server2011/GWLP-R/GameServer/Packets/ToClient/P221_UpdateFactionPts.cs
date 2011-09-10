using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 221)]
        public class P221_UpdateFactionPts : IPacket
        {
                public class PacketSt221 : IPacketTemplate
                {
                        public UInt16 Header { get { return 221; } }
                        public UInt32 ExpPts;
                        public UInt32 KurzFree;
                        public UInt32 KurzTotal;
                        public UInt32 LuxFree;
                        public UInt32 LuxTotal;
                        public UInt32 ImpFree;
                        public UInt32 ImpTotal;
                        public UInt32 Data1;
                        public UInt32 Data2;
                        public UInt32 Level;
                        public UInt32 Morale;
                        public UInt32 BalthFree;
                        public UInt32 BalthTotal;
                        public UInt32 SkillFree;
                        public UInt32 SkillTotal;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt221>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt221)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt221> pParser;

        }
}
