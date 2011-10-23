using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 148)]
        public class P148_SwitchMode : IPacket
        {
                public class PacketSt148 : IPacketTemplate
                {
                        public UInt16 Header { get { return 148; } }
                        public byte bHM;//0 = NM, 1 = HM
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt148>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt148> pParser;
        }
}
