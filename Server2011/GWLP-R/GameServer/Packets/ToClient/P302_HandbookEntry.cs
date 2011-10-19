using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 302)]
        public class P302_HandbookEntry : IPacket
        {
                public class PacketSt302 : IPacketTemplate
                {
                        public UInt16 Header { get { return 302; } }
                        public UInt32 Data1; //sort of handbook id
                        public byte HandbookEntry; // 0 based
                        [PacketFieldType(ConstSize = false, MaxSize = 8)]
                        public string Data3;
                        [PacketFieldType(ConstSize = false, MaxSize = 8)]
                        public string Data4;
                        [PacketFieldType(ConstSize = false, MaxSize = 8)]
                        public string Data5;
                        [PacketFieldType(ConstSize = false, MaxSize = 8)]
                        public string Data6;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt302>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt302)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt302> pParser;

        }
}
