using System;
using LoginServer.Packets.ToGameServer;
using LoginServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65281)]
        public class P65281_HandshakeRequest : IPacket
        {
                public class PacketSt65281 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65281; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 8)]
                        public byte[] SecurityKey1;
                        [PacketFieldType(ConstSize = true, MaxSize = 8)]
                        public byte[] SecurityKey2;
                        public UInt32 Port;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65281>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt65281();
                        pParser((PacketSt65281)message.PacketTemplate, message.PacketData);

#warning No security check here

                        // we've got a new game server, lets get the network data of it:
                        byte[] ip;
                        int port;
                        if (NetworkManager.Instance.GetClientInfo(message.NetID, out ip, out port))
                        {
                                // if the client is valid (or even existing)
                                // Note: use the port from the packet here, because we dont want to have the port that the game server uses to connect to us
                                var server = new GameServer(message.NetID, ip, (int)((PacketSt65281)message.PacketTemplate).Port);

                                World.AddGameServer(server);

                                // create reply
                                var reply = new NetworkMessage(message.NetID)
                                                    {PacketTemplate = new P65281_HandshakeReply.PacketSt65281()};
                                QueuingService.PostProcessingQueue.Enqueue(reply);
                        }



                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65281> pParser;
        }
}
