using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 112)]
        public class P112_SalvageItem : IPacket
        {
                public class PacketSt112 : IPacketTemplate
                {
                        public UInt16 Header { get { return 112; } }
                        public UInt16 ItemStream;
                        public UInt32 KitID;
                        public UInt32 ItemID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt112>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt112> pParser;
        }
}
