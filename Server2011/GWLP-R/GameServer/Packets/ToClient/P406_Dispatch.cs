using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 406)]
        public class P406_Dispatch : IPacket
        {
                public class PacketSt406 : IPacketTemplate
                {
                        public UInt16 Header { get { return 406; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] ConnectionInfo;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        public byte Region;
                        public UInt16 ZoneID;
                        public byte IsOutpost;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt406>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt406)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt406> pParser;

        }
}
