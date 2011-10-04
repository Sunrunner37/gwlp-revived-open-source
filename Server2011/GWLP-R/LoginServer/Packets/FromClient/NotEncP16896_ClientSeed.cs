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
        [PacketAttributes(IsIncoming = true, Header = 16896)]
        class NotEncP16896_ClientSeed : IPacket
        {
                public class PacketSt16896 : IPacketTemplate
                {
                        public UInt16 Header { get { return 16896; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 64)]
                        public byte[] Seed;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt16896>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt16896();
                        pParser(pack, message.PacketData);

                        // check the sync state of the client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);

                        if (client.Data.Status == SyncStatus.ConnectionEstablished)
                        {
                                client.Data.EncryptionSeed = pack.Seed;

                                // Note: SERVER SEED
                                var msg = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new NotEncP5633_ServerSeed.PacketSt5633
                                        {
                                                Seed = new byte[20]
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                return true;
                        }

                        // if the client is in any different sync state, kick it
                        LoginServerWorld.Instance.Kick(client);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt16896> pParser;
        }
}
