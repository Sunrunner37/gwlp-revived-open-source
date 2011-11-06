using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 328)]
        public class P328_SwapItems : IPacket
        {
                public class PacketSt328 : IPacketTemplate
                {
                        public UInt16 Header { get { return 328; } }
                        public UInt16 ItemStreamID;
                        public UInt32 MovedItemLocalID;
                        public UInt32 ItemToBeSwappedWithLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt328>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt328)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt328> pParser;

        }
}
