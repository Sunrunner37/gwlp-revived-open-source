using System;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 9)]
        public class P09_LastCharInfo : IPacket
        {
                public class PacketSt9 : IPacketTemplate
                {
                        public UInt16 Header { get { return 9; } }
                        public UInt32 LoginCount;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Data2;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public byte[] Data3;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt9>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt9();
                        pParser(pack, message.PacketData);

                        // get the client
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

                private PacketParser<PacketSt9> pParser;
        }
}
