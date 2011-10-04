using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 329)]
        public class Packet329 : IPacket
        {
                public class PacketSt329 : IPacketTemplate
                {
                        public UInt16 Header { get { return 329; } }
                        public UInt16 Data1;
                        public UInt32 Data2;
                        public UInt16 Data3;
                        public byte Data4;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt329>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt329)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt329> pParser;

        }
}
