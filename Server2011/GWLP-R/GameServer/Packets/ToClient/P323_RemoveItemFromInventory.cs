using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 323)]
        public class P323_RemoveItemFromInventory : IPacket
        {
                public class PacketSt323 : IPacketTemplate
                {
                        public UInt16 Header { get { return 323; } }
                        public UInt16 ItemStreamID;
                        public UInt32 ItemLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt323>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt323)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt323> pParser;

        }
}
