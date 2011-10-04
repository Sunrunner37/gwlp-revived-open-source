using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 395)]
        public class P395_InstanceLoadDistrictInfo : IPacket
        {
                public class PacketSt395 : IPacketTemplate
                {
                        public UInt16 Header { get { return 395; } }
                        public UInt32 LocalID;
                        public UInt16 GameMapID;
                        public byte IsOutpost;
                        public UInt16 DistrictNumber;
                        public UInt16 DistrictRegion; 
                        public byte Data1;
                        public byte ObserverMode;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt395>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt395)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt395> pParser;

        }
}
