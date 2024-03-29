using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 304)]
        public class P304_ItemOwnerName : IPacket
        {
                public class PacketSt304 : IPacketTemplate
                {
                        public UInt16 Header { get { return 304; } }
                        public UInt32 ItemLocalID;
                        [PacketFieldType(ConstSize = false, MaxSize = 32)]
                        public string CharName; //NO gw string
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt304>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt304)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt304> pParser;

        }
}
