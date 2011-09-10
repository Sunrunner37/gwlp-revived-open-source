using System;
using LoginServer.Enums;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 13)]
        public class P13_AccountLogout : IPacket
        {
                public class PacketSt13 : IPacketTemplate
                {
                        public UInt16 Header { get { return 13; } }
                        public UInt32 Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt13>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt13();
                        pParser((PacketSt13)message.PacketTemplate, message.PacketData);

                        Client client;
                        lock (client = World.GetClient(Idents.Clients.NetID, message.NetID))
                        {
                                if (client.Status != SyncState.PossibleQuit)
                                {
                                        client.LoginCount++;
                                        client.Email = "";
                                        client.Password = "";
                                        // reset status
                                        client.Status = SyncState.EncryptionEstablished;
                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt13> pParser;
        }
}
