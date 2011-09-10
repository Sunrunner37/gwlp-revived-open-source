using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToGameServer
{
        [PacketAttributes(IsIncoming = false, Header = 65283)]
        public class P65283_BuildMapRequest : IPacket
        {
                public class PacketSt65283 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65283; } }
                        public UInt32 MapID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65283>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65283)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65283> pParser;
        }
}
