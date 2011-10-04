using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 30)]
        public class P030_MovementAim : IPacket
        {
                public class PacketSt30 : IPacketTemplate
                {
                        public UInt16 Header { get { return 30; } }
                        public UInt32 AgentID;
                        public Single X;
                        public Single Y;
                        public UInt16 PlaneZ;
                        public UInt16 Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt30>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt30)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt30> pParser;

        }
}
