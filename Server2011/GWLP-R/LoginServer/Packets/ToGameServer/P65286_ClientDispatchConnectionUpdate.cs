using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToGameServer
{
        [PacketAttributes(IsIncoming = false, Header = 65286)]
        public class P65286_ClientDispatchConnectionUpdate : IPacket
        {
                public class PacketSt65286 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65286; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] ConnectionInfo;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                        public UInt32 OldMapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65286>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt65286)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65286> pParser;
        }
}
