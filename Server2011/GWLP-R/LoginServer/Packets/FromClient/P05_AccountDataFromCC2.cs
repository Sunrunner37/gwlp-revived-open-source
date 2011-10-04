using System;
using LoginServer.Enums;
using LoginServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 5)]
        public class P05_AccountDataFromCC2 : IPacket
        {
                public class PacketSt5 : IPacketTemplate
                {
                        public UInt16 Header { get { return 5; } }
                        public UInt32 LoginCount;
                        public UInt32 Data2;
                        [PacketFieldType(ConstSize = true, MaxSize = 20)]
                        public byte[] Data3;
                        [PacketFieldType(ConstSize = true, MaxSize = 20)]
                        public byte[] Data4;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public string Data5;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt5>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt5();
                        pParser(pack, message.PacketData);

                        // get the client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // update the sync counter (aka login count)
                        client.Data.SyncCount = pack.LoginCount;
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt5> pParser;
        }
}
