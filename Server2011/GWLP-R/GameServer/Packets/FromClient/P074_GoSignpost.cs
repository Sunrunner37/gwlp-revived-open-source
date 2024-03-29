using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 74)]
        public class P074_GoSignpost : IPacket
        {
                public class PacketSt74 : IPacketTemplate
                {
                        public UInt16 Header { get { return 74; } }
                        public UInt32 AgentID;
                        public byte Flag;//0
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt74>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt74> pParser;
        }
}
