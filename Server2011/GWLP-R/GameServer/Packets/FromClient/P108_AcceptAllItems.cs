using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 108)]
        public class P108_AcceptAllItems : IPacket
        {
                public class PacketSt108 : IPacketTemplate
                {
                        public UInt16 Header { get { return 108; } }
                        public UInt16 BagID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt108>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt108> pParser;
        }
}
