using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 170)]
        public class P170_RequestMapChange : IPacket
        {
                public class PacketSt170 : IPacketTemplate
                {
                        public UInt16 Header { get { return 170; } }
                        public UInt16 MapID;
                        public byte Region;
                        public UInt16 District;
                        public byte Language;
                        public byte Flag;//1
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt170>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt170> pParser;
        }
}
