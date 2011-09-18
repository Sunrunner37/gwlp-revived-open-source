using System;
using System.IO;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.DataBase;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65284)]
        public class P65284_AcceptPlayerReply : IPacket
        {
                public class PacketSt65284 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65284; } }
                        public UInt32 AccID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65284>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt65284();
                        pParser((PacketSt65284)message.PacketTemplate, message.PacketData);

                        // get client and send refer to gameserver
                        var client = World.GetClient(Idents.Clients.AccID, ((PacketSt65284) message.PacketTemplate).AccID);
                        
                        var gameServer = World.GetGameServer(Idents.GameServers.NetID, message.NetID);
                                
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // send refer to game server:
                                var msg = new NetworkMessage((int)client[Idents.Clients.NetID])
                                {
                                        PacketTemplate = new P09_ReferToGameServer.PacketSt9()
                                };
                                // set the message data
                                ((P09_ReferToGameServer.PacketSt9)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                ((P09_ReferToGameServer.PacketSt9)msg.PacketTemplate).GameMapID = (ushort)(from m in db.mapsMasterData where m.gameMapID == client.MapID select m).First().gameMapID;
                                ((P09_ReferToGameServer.PacketSt9)msg.PacketTemplate).SecurityKey1 = client.SecurityKeys[0];
                                ((P09_ReferToGameServer.PacketSt9)msg.PacketTemplate).SecurityKey2 = client.SecurityKeys[1];
                                // set the server ip and port
                                var ms = new MemoryStream();
                                RawConverter.WriteUInt16(2, ms); // this is necessary, whatever it does.
                                RawConverter.WriteByteAr(BitConverter.GetBytes((short)(int)gameServer[Idents.GameServers.Port]).Reverse().ToArray(), ms);
                                RawConverter.WriteByteAr((byte[])gameServer[Idents.GameServers.IP], ms);
                                var serverConnection = new byte[24];
                                ms.ToArray().CopyTo(serverConnection, 0);
                                // add it
                                ((P09_ReferToGameServer.PacketSt9)msg.PacketTemplate).ServerConnectionInfo = serverConnection;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(msg);
                        }
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65284> pParser;
        }
}
