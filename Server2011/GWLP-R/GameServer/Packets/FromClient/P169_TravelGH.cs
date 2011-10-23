using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 169)]
        public class P169_TravelGH : IPacket
        {
                public class PacketSt169 : IPacketTemplate
                {
                        public UInt16 Header { get { return 169; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] GuildGUID;
                        public byte Flag;// 1
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt169>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt169> pParser;
        }
}
