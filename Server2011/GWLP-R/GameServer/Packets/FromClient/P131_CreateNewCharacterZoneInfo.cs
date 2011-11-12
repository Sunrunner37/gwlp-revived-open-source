using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 131)]
        public class P131_CreateNewCharacterZoneInfo : IPacket
        {
                public class PacketSt131 : IPacketTemplate
                {
                        public UInt16 Header { get { return 131; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt131>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        //Zoning information

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt131> pParser;
        }
}
