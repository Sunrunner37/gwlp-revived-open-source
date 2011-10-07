using System;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 2)]
        public class P002_ClientExitGame : IPacket
        {
                public class PacketSt2 : IPacketTemplate
                {
                        public UInt16 Header { get { return 2; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt2>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // nothing to parse here

                        // get the client
                        var client = GameServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // check if it tries to change the game server
                        if (client.Data.Status == SyncStatus.Dispatching)
                        {
                                // we cannot simply kick it then ;)
#warning CHECKME Does this also work when we terminate the connection?
                                NetworkManager.Instance.PauseClient(message.NetID);
                        }
                        else
                        {
                                GameServerWorld.Instance.Kick(client);
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt2> pParser;
        }
}
