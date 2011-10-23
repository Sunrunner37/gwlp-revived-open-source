using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 31)]
        public class P31_AttackTarget : IPacket
        {
                public class PacketSt31 : IPacketTemplate
                {
                        public UInt16 Header { get { return 31; } }
                        public UInt32 TargetID;
                        public byte Flag;//0 for Attack 1 for Ping
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt31>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt31> pParser;
        }
}
