using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 9)]
        public class P09_ReferToGameServer : IPacket
        {
                public class PacketSt9 : IPacketTemplate
                {
                        public UInt16 Header { get { return 9; } }
                        public UInt32 LoginCount;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] SecurityKey1;
                        public UInt32 GameMapID;
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] ServerConnectionInfo;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] SecurityKey2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt9>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt9)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt9> pParser;

        }
}
