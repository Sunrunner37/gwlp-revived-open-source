using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 58)]
        public class P58_ChangeSecondProfession : IPacket
        {
                public class PacketSt58 : IPacketTemplate
                {
                        public UInt16 Header { get { return 58; } }
                        public UInt32 AgentID;
                        public byte Profession;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt58>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt58> pParser;
        }
}
