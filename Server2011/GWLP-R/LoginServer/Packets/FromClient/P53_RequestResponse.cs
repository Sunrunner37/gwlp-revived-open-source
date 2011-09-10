using System;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
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
                        message.PacketTemplate = new PacketSt53();
                        pParser((PacketSt53)message.PacketTemplate, message.PacketData);

                        Client client;
                        lock (client = World.GetClient(Idents.Clients.NetID, message.NetID))
                        {
                                client.LoginCount = (int)((PacketSt53)message.PacketTemplate).LoginCount;

                                // send a response (whatever that does):
                                var sendResponse = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P38_SendResponse.PacketSt38()
                                };
                                // set the message data
                                ((P38_SendResponse.PacketSt38)sendResponse.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                ((P38_SendResponse.PacketSt38)sendResponse.PacketTemplate).Data1 = 0;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(sendResponse);

                                // send a stream terminator:
                                var streamTerminator = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                };
                                // set the message data
                                ((P03_StreamTerminator.PacketSt3)streamTerminator.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                ((P03_StreamTerminator.PacketSt3)streamTerminator.PacketTemplate).ErrorCode = 0;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(streamTerminator);
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt53> pParser;
        }
}
