using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 154)]
        public class P154_UpdatePublicProfessions : IPacket
        {
                public class PacketSt154 : IPacketTemplate
                {
                        public UInt16 Header { get { return 154; } }
                        public UInt32 ID1;
                        public byte Prof1;
                        public byte Prof2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt154>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt154)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt154> pParser;

        }
}
