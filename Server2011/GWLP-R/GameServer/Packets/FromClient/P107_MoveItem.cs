using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 107)]
        public class P107_MoveItem : IPacket
        {
                public class PacketSt107 : IPacketTemplate
                {
                        public UInt16 Header { get { return 107; } }
                        public UInt32 ItemID;
                        public UInt16 BagID;
                        public byte Slot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt107>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt107> pParser;
        }
}
