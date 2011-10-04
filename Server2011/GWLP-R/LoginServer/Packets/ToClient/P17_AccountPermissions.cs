using System;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.ToClient
{
        [PacketAttributes(IsIncoming = false, Header = 17)]
        public class P17_AccountPermissions : IPacket
        {
                public class PacketSt17 : IPacketTemplate
                {
                        public UInt16 Header { get { return 17; } }
                        public UInt32 LoginCount;
                        public UInt32 Territory; // 2 europe, 0 america
                        public UInt32 TerritoryChanges;
                        [PacketFieldType(ConstSize = true, MaxSize = 8)]
                        public byte[] Data1;
                        [PacketFieldType(ConstSize = true, MaxSize = 8)]
                        public byte[] Data2;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] Data3;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] Data4;
                        public UInt32 ChangeAccSettings; // 2 = change: email, pw, mail add
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 200)]
                        public byte[] AddedKeys; // short: keyID, short: flag
                        public byte EulaAccepted;
                        public UInt32 Data5;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt17>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        pParser((PacketSt17)message.PacketTemplate, message.PacketData);
                        QueuingService.NetOutQueue.Enqueue(message);
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt17> pParser;

        }
}
