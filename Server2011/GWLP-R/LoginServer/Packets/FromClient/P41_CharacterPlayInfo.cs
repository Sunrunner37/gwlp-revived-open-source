using System;
using LoginServer.Packets.ToClient;
using LoginServer.Packets.ToGameServer;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
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
                        public UInt32 LoginCount;
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
                        var pack = new PacketSt41();
                        pParser(pack, message.PacketData);

                        // get the client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // update the login counter
                        client.Data.SyncCount = pack.LoginCount;

                        // get the most suitable game server
                        DataGameServer server;
                        if (!LoginServerWorld.Instance.GetBestGameServer(client.Data.MapID, out server) && server != null)
                        {
                                // if we've got a server but no map, build one.
                                // Note: BUILD MAP REQUEST
                                var buildMap = new NetworkMessage(server.Data.NetID)
                                {
                                        PacketTemplate =new P65283_BuildMapRequest.PacketSt65283
                                        {
                                                MapID = client.Data.MapID.Value
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(buildMap);

                        }
                        else if (server == null)
                        {
                                // no game server found,
                                // Note: STREAM TERMINATOR
                                var msg = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P03_StreamTerminator.PacketSt3
                                        {
                                                LoginCount = client.Data.SyncCount,
                                                ErrorCode = 5,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                return true;
                        }

                        // if we've got a server and a map, let the game server accept a new player
                        // Note: ACCEPT PLAYER REQUEST
                        var acceptPlayer = new NetworkMessage(server.Data.NetID)
                        {
                                PacketTemplate = new P65284_AcceptPlayerRequest.PacketSt65284
                                {
                                        AccID = client.Data.AccID.Value,
                                        CharID = client.Data.CharID.Value,
                                        MapID = client.Data.MapID.Value,
                                        Key1 = client.Data.SecurityKeys[0],
                                        Key2 = client.Data.SecurityKeys[1],
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(acceptPlayer);
                                
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt41> pParser;
        }
}
