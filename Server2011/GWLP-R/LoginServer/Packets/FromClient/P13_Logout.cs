using System;
using LoginServer.Enums;
using LoginServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 13)]
        public class P13_Logout : IPacket
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
                        var pack = new PacketSt13();
                        pParser(pack, message.PacketData);

                        // get client
                        var client = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        if (client.Data.Status != SyncStatus.PossibleQuit)
                        {
                                client.Data.SyncCount++;
                                client.Data.Email = "";
                                client.Data.Password = "";
                                // reset status
                                client.Data.Status = SyncStatus.EncryptionEstablished;
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt13> pParser;
        }
}
