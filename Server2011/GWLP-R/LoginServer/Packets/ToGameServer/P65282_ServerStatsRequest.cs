using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToGameServer
{
        [PacketAttributes(IsIncoming = false, Header = 65282)]
        public class P65282_ServerStatsRequest : IPacket
        {
                public class PacketSt65282 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65282; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65282>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65282)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65282> pParser;
        }
}
