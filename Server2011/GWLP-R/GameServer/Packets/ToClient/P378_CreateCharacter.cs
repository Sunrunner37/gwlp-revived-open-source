using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 378)]
        public class P378_CreateCharacter : IPacket
        {
                public class PacketSt378 : IPacketTemplate
                {
                        public UInt16 Header { get { return 378; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] StaticHash;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharName;
                        public UInt16 GameMapID;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 1024)]
                        public byte[] Appearance;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt378>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt378)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt378> pParser;

        }
}
