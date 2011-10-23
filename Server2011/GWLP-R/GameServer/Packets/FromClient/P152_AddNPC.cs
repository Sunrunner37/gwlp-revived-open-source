using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 152)]
        public class P152_AddNPC : IPacket
        {
                public class PacketSt152 : IPacketTemplate
                {
                        public UInt16 Header { get { return 152; } }
                        public UInt16 NPCID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt152>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt152> pParser;
        }
}
