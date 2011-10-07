using System;
using System.IO;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.StaticConvert;
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
                        public byte Success;
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
                        var pack = new PacketSt65284();
                        pParser(pack, message.PacketData);

                        // get the client
                        var client = LoginServerWorld.Instance.Get<DataClient>(new AccID(pack.AccID));

                        // check if the game server successfully added it
                        if (pack.Success == 0)
                        {
                                // kick the client, cause the game server couldnt add it
                                LoginServerWorld.Instance.Kick(client);
                                return true;
                        }
                        
                        // get the game server that accepted the client
                        var gameServer = LoginServerWorld.Instance.Get<DataGameServer>(message.NetID);
                                
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // prepare the connection array
                                var ms = new MemoryStream();
                                RawConverter.WriteUInt16(2, ms); // this is necessary, whatever it does.
                                // port
                                RawConverter.WriteByteAr(BitConverter.GetBytes((short)gameServer.Data.Port.Value).Reverse().ToArray(), ms);
                                // ip
                                RawConverter.WriteByteAr(gameServer.Data.IPAddress.Value, ms);
                                // static data
                                var serverConnection = new byte[24];
                                ms.ToArray().CopyTo(serverConnection, 0);

                                // send refer to game server:
                                // Note: REFER TO GAMESERVER
                                var msg = new NetworkMessage(client.Data.NetID)
                                {
                                        PacketTemplate = new P09_ReferToGameServer.PacketSt9
                                        {
                                                LoginCount = client.Data.SyncCount,
                                                GameMapID = (ushort)(from m in db.mapsMasterData where m.mapID == client.Data.MapID.Value select m).First().gameMapID,
                                                SecurityKey1 = client.Data.SecurityKeys[0],
                                                SecurityKey2 = client.Data.SecurityKeys[1],
                                                ServerConnectionInfo = serverConnection,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(msg);
                        }
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65284> pParser;
        }
}
