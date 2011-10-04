using System;
using LoginServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 33)]
        public class P33_AccountDataFromCC : IPacket
        {
                public class PacketSt33 : IPacketTemplate
                {
                        public UInt16 Header { get { return 33; } }
                        public UInt32 LoginCount;
                        public UInt32 Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt33>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt33();
                        pParser(pack, message.PacketData);

                        // get the client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // update the sync count
                        client.Data.SyncCount = pack.LoginCount;
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt33> pParser;
        }
}
