using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 150)]
        public class P150_UpdateGenericValueFloat : IPacket
        {
                public class PacketSt150 : IPacketTemplate
                {
                        public UInt16 Header { get { return 150; } }
                        public UInt32 ValueID;
                        public UInt32 ID1;
                        public float Value; // was UInt32
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt150>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt150)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt150> pParser;

        }
}
