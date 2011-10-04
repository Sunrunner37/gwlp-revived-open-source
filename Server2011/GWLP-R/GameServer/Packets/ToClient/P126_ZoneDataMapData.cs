using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 126)]
        public class P126_ZoneDataMapData : IPacket
        {
                public class PacketSt126 : IPacketTemplate
                {
                        public UInt16 Header { get { return 126; } }
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 256)]
                        public byte[] Data1; // was UInt32[]
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt126>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt126)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt126> pParser;

        }
}
