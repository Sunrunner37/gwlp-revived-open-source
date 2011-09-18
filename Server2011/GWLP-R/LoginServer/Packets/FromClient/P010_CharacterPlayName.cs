using System;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.DataBase;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 10)]
        public class P010_CharacterPlayName : IPacket
        {
                public class PacketSt10 : IPacketTemplate
                {
                        public UInt16 Header { get { return 10; } }
                        public UInt32 LoginCount;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Name;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt10>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt10();
                        pParser((PacketSt10)message.PacketTemplate, message.PacketData);

                        var client = World.GetClient(Idents.Clients.NetID, message.NetID);
                        
                        client.LoginCount = (int)((PacketSt10) message.PacketTemplate).LoginCount;

                        // Get the charID of a char with that name
                        var charID = 0;
                        var mapID = 0;
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var name = ((PacketSt10) message.PacketTemplate).Name;
                                var accID = (int) client[Idents.Clients.AccID];

                                var chara = (from c in db.charsMasterData
                                                where c.charName == name && c.accountID == accID
                                                select c).First();

                                charID = chara.charID;
                                mapID = chara.mapID;
                        }

                        // update the client with new charID
                        var newClient = new Client(message.NetID, (int)client[Idents.Clients.AccID], charID)
                        {
                                Email = client.Email,
                                InitCryptSeed = client.InitCryptSeed,
                                LoginCount = client.LoginCount,
                                Password = client.Password,
                                SecurityKeys = client.SecurityKeys,
                                Status = SyncState.AtCharView,
                                MapID = mapID,
                        };

                        World.UpdateClient(client, newClient);

                        // send a friendslist end packet:
                        var friendsListEnd = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P20_FriendsListEnd.PacketSt20()
                        };
                        // add data
                        ((P20_FriendsListEnd.PacketSt20)friendsListEnd.PacketTemplate).LoginCount = (uint)newClient.LoginCount;
                        ((P20_FriendsListEnd.PacketSt20)friendsListEnd.PacketTemplate).StaticData1 = 1;
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(friendsListEnd);

                        // send a acc permissions packet:
                        var accountPermissions = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P17_AccountPermissions.PacketSt17()
                        };
                        // add data
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).LoginCount = (uint)newClient.LoginCount;
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Territory = 2; // sd america
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).TerritoryChanges = 4;
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Data1 = new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Data2 = new byte[] { 0x80, 0x3F, 0x02, 0x00, 0x03, 0x00, 0x08, 0x00 };
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Data3 = new byte[] { 0x37, 0x4B, 0x09, 0xBB, 0xC2, 0xF6, 0x74, 0x43, 0xAA, 0xAB, 0x35, 0x4D, 0xEE, 0xB7, 0xAF, 0x08 };
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Data4 = new byte[] { 0x55, 0xB6, 0x77, 0x59, 0x0C, 0x0C, 0x15, 0x46, 0xAD, 0xAA, 0x33, 0x43, 0x4A, 0x91, 0x23, 0x6A };
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).ChangeAccSettings = 8;
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).ArraySize1 = 8;
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).AddedKeys = new byte[] { 0x01, 0x00, 0x06, 0x00, 0x57, 0x00, 0x01, 0x00 };
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).EulaAccepted = 23;
                        ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).Data5 = 0;
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(accountPermissions);

                        // send a stream terminator:
                        var msg = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P03_StreamTerminator.PacketSt3()
                        };
                        // set the message data
                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)newClient.LoginCount;
                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = 0;
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(msg);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt10> pParser;
        }
}
