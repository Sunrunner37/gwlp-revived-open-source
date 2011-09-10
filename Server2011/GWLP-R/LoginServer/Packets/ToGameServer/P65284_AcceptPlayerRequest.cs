using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToGameServer
{
        [PacketAttributes(IsIncoming = false, Header = 65284)]
        public class P65284_AcceptPlayerRequest : IPacket
        {
                public class PacketSt65284 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65284; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65284>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65284)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65284> pParser;
        }
}
