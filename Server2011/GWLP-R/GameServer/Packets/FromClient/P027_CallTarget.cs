using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 27)]
        public class P027_CallTarget : IPacket
        {
                public class PacketSt27 : IPacketTemplate
                {
                        public UInt16 Header { get { return 27; } }
                        public byte Flag;//0xA
                        public UInt32 TargetID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt27>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt27> pParser;
        }
}
