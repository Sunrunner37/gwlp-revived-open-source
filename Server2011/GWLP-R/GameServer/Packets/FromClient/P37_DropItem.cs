using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 37)]
        public class P37_DropItem : IPacket
        {
                public class PacketSt37 : IPacketTemplate
                {
                        public UInt16 Header { get { return 37; } }
                        public UInt32 ItemID;
                        public byte Amount;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt37>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt37> pParser;
        }
}
