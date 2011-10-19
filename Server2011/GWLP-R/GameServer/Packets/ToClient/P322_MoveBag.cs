using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 322)]
        public class P322_MoveBag : IPacket
        {
                // this packet moves bags.. as there are only 2 places for the same bag to be (StorageID: 2 and 3) its basically
                // just for swapping bags between 2 and 3

                public class PacketSt322 : IPacketTemplate
                {
                        public UInt16 Header { get { return 322; } }
                        public UInt16 ItemStreamID;
                        public byte FromStorageID; // enum ItemStorage
                        public byte ToStorageID; // enum ItemStorage
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt322>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt322)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt322> pParser;

        }
}
