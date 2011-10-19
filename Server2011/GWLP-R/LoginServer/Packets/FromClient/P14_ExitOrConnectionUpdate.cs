using System;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
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
                        var pack = new PacketSt14();
                        pParser(pack, message.PacketData);
                        
                        var client= LoginServerWorld.Instance.Get<DataClient>(message.NetID);

                        // check if it already disconnected
                        if (client == null) return true;

                        switch (pack.ExitCode)
                        {
                                // possible exit
                                case 0:
                                        {
                                                client.Data.Status = SyncStatus.PossibleQuit;

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
                                        }
                                        break;
                                // loads instance
                                case 1:
                                        {
                                                client.Data.Status = SyncStatus.TriesToLoadInstance;
                                        }
                                        break;
                                // strange stuff, kick client
                                default:
                                        {
                                                // the client has send strange data, so kick it
                                                LoginServerWorld.Instance.Kick(client);
                                        }
                                        break;
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt14> pParser;
        }
}
