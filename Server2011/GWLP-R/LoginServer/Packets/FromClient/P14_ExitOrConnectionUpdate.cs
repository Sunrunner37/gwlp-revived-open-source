using System;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 14)]
        public class P14_ExitOrConnectionUpdate : IPacket
        {
                public class PacketSt14 : IPacketTemplate
                {
                        public UInt16 Header { get { return 14; } }
                        public UInt32 ExitCode;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt14>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt14();
                        pParser((PacketSt14)message.PacketTemplate, message.PacketData);
                        
                        // 0=exit
                        Client client;
                        switch (((PacketSt14)message.PacketTemplate).ExitCode)
                        {
                                case 0:
                                        lock (client = World.GetClient(Idents.Clients.NetID, message.NetID))
                                        {
                                                client.Status = SyncState.PossibleQuit;

                                                var msg = new NetworkMessage(message.NetID);
                                                msg.PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                                {
                                                        LoginCount = (uint)client.LoginCount,
                                                        ErrorCode = 0
                                                };
                                                QueuingService.PostProcessingQueue.Enqueue(msg);
                                                
                                        }
                                        break;
                                case 1:
                                        lock (client = World.GetClient(Idents.Clients.NetID, message.NetID))
                                        {
                                                client.Status = SyncState.TriesToLoadInstance;
                                        }
                                        break;
                                default:
                                        throw new NotImplementedException();
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt14> pParser;
        }
}
