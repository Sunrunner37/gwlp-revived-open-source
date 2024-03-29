using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 149)]
        public class Packet149 : IPacket
        {
                public class PacketSt149 : IPacketTemplate
                {
                        public UInt16 Header { get { return 149; } }
                        public Single Data1;
                        public Single Data2;
                        public UInt16 Data3;
                        public UInt32 ID1;
                        public UInt16 Data4;
                        public byte Data5;
                        public byte Data6;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt149>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt149)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt149> pParser;

        }
}
