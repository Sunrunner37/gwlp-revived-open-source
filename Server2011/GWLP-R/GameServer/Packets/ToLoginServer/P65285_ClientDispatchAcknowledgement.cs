using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToLoginServer
{
        [PacketAttributes(IsIncoming = false, Header = 65285)]
        public class P65285_ClientDispatchAcknowledgement : IPacket
        {
                public class PacketSt65285 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65285; } }
                        public UInt32 AccID;
                        public UInt32 MapID;
                        public UInt32 OldMapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65285>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65285)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65285> pParser;
        }
}
