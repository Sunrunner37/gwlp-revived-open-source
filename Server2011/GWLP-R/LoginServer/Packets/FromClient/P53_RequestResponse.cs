using System;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 53)]
        public class P53_RequestResponse : IPacket
        {
                public class PacketSt53 : IPacketTemplate
                {
                        public UInt16 Header { get { return 53; } }
                        public UInt32 LoginCount;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt53>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt53();
                        pParser(pack, message.PacketData);

                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        client.Data.SyncCount = pack.LoginCount;

                        // send a response (whatever that does):
                        // Note: LS SEND RESPONSE
                        var sendResponse = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P38_SendResponse.PacketSt38
                                {
                                        LoginCount = client.Data.SyncCount,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(sendResponse);

                        // send a stream terminator:
                        // Note: STREAM TERMINATOR
                        var streamTerminator = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P03_StreamTerminator.PacketSt3
                                {
                                        LoginCount = client.Data.SyncCount,
                                        ErrorCode = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(streamTerminator);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt53> pParser;
        }
}
