using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 7)]
        public class P07_CharacterInfo : IPacket
        {
                public class PacketSt7 : IPacketTemplate
                {
                        public UInt16 Header { get { return 7; } }
                        public UInt32 LoginCount;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] StaticHash1;
                        public UInt32 StaticData1;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharName;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public byte[] Appearance;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt7>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt7)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt7> pParser;

        }
}
