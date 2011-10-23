using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 114)]
        public class P114_DialogButton : IPacket
        {
                public class PacketSt114 : IPacketTemplate
                {
                        public UInt16 Header { get { return 114; } }
                        public byte Icon;
                        [PacketFieldType(ConstSize = false, MaxSize = 128)]
                        public string Text;
                        public UInt32 ButtonId;//client will sent this when button is clicked
                        public UInt32 Data4;//mostly -1
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt114>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt114)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt114> pParser;

        }
}
