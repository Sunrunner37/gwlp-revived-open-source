using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 117)]
        public class P117_DialogSender : IPacket
        {
                public class PacketSt117 : IPacketTemplate
                {
                        public UInt16 Header { get { return 117; } }
                        public UInt32 AgentID; // the agent sending the dialog. 0 = nobody specific.
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt117>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt117)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt117> pParser;

        }
}
