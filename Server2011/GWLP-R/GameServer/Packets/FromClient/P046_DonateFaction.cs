using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 46)]
        public class P046_DonateFaction : IPacket
        {
                public class PacketSt46 : IPacketTemplate
                {
                        public UInt16 Header { get { return 46; } }
                        public UInt32 Flag;//0
                        public byte Faction;
                        public UInt32 Amount;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt46>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt46> pParser;
        }
}
