using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 5633)]
        public class NotEncP5633_ServerSeed : IPacket
        {
                public class PacketSt5633 : IPacketTemplate
                {
                        public UInt16 Header { get { return 5633; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 20)]
                        public byte[] Seed;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt5633>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt5633)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt5633> pParser;

        }
}
