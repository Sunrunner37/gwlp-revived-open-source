using System;
using System.Diagnostics;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
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
                        message.PacketTemplate = new PacketSt16896();
                        pParser((PacketSt16896)message.PacketTemplate, message.PacketData);

                        // check the sync state of the client
                        var client = World.GetClient(Idents.Clients.NetID, message.NetID);

                        lock (client)
                        {
                                if (client.Status == SyncState.ConnectionEstablished)
                                {
                                        client.InitCryptSeed = ((PacketSt16896) message.PacketTemplate).Seed;

                                        // send server seed:
                                        //
                                        var msg = new NetworkMessage(message.NetID);
                                        // set the message type
                                        msg.PacketTemplate = new NotEncP5633_ServerSeed.PacketSt5633();
                                        // set the message data
                                        ((NotEncP5633_ServerSeed.PacketSt5633)msg.PacketTemplate).Seed = new byte[20];
                                        // send it
                                        QueuingService.PostProcessingQueue.Enqueue(msg);

                                        return true;
                                }
                                // if the client is in any different sync state, kick it
                                World.KickClient(Idents.Clients.NetID, message.NetID);
                        }
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt16896> pParser;
        }
}
