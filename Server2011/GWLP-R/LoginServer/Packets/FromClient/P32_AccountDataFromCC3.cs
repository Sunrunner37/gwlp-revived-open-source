using System;
using System.IO;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 32)]
        public class P32_AccountDataFromCC3 : IPacket
        {
                public class PacketSt32 : IPacketTemplate
                {
                        public UInt16 Header { get { return 32; } }
                        public UInt32 LoginCount;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 512)]
                        public byte[] Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt32>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt32();
                        pParser(pack, message.PacketData);

                        // get client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // update the sync counter
                        client.Data.SyncCount = pack.LoginCount;

                        // Note: STREAM TERMINATOR
                        var msg = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                {
                                        LoginCount = client.Data.SyncCount,
                                        ErrorCode = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(msg);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt32> pParser;
        }
}
