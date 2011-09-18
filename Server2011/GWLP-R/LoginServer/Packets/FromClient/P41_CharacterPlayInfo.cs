using System;
using LoginServer.Packets.ToClient;
using LoginServer.Packets.ToGameServer;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 41)]
        public class P41_CharacterPlayInfo : IPacket
        {
                public class PacketSt41 : IPacketTemplate
                {
                        public UInt16 Header { get { return 41; } }
                        public UInt32 Data1;
                        public UInt32 Data2;
                        public UInt32 Data3;
                        public UInt32 Data4;
                        public UInt32 Data5;
                        public UInt32 Data6;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt41>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt41();
                        pParser((PacketSt41)message.PacketTemplate, message.PacketData);

                        var client = World.GetClient(Idents.Clients.NetID, message.NetID);
                        
                        // necessary!
#warning CHECKME Character play info should have login count...
                        client.LoginCount++;

                        var serverNetID = 0;
                        GameServer server;
                        if (!World.GetBestGameServer(client.MapID, out server) && server != null)
                        {
                                serverNetID = (int) server[Idents.GameServers.NetID];


                                var buildMap = new NetworkMessage(serverNetID)
                                                {
                                                        PacketTemplate =new P65283_BuildMapRequest.PacketSt65283()
                                                };
                                // set the message data
                                ((P65283_BuildMapRequest.PacketSt65283) buildMap.PacketTemplate).MapID = (uint) client.MapID;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(buildMap);

                        }
                        else if (server == null)
                        {
                                // no game server found,
                                // send a stream terminator:
                                var msg = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                };
                                // set the message data
                                ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = 5;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                return true;
                        }

                        // if the map already exists, the netID was not yet set:
                        serverNetID = (int)server[Idents.GameServers.NetID];

                        var acceptPlayer = new NetworkMessage(serverNetID)
                        {
                                PacketTemplate = new P65284_AcceptPlayerRequest.PacketSt65284()
                        };
                        // set the message data
                        ((P65284_AcceptPlayerRequest.PacketSt65284)acceptPlayer.PacketTemplate).AccID = (uint)((int)client[Idents.Clients.AccID]);
                        ((P65284_AcceptPlayerRequest.PacketSt65284)acceptPlayer.PacketTemplate).CharID = (uint)((int)client[Idents.Clients.CharID]);
                        ((P65284_AcceptPlayerRequest.PacketSt65284)acceptPlayer.PacketTemplate).MapID = (uint)client.MapID;
                        ((P65284_AcceptPlayerRequest.PacketSt65284)acceptPlayer.PacketTemplate).Key1 = client.SecurityKeys[0];
                        ((P65284_AcceptPlayerRequest.PacketSt65284)acceptPlayer.PacketTemplate).Key2 = client.SecurityKeys[1];
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(acceptPlayer);
                                

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt41> pParser;
        }
}
