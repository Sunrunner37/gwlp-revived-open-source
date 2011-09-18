using System;
using System.IO;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.DataBase;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 4)]
        public class P04_AccountLogin : IPacket
        {
                public class PacketSt4 : IPacketTemplate
                {
                        public UInt16 Header { get { return 4; } }
                        public UInt32 LoginCount;
                        //public UInt32 PwSize;
                        //[PacketFieldType(ConstSize = true, MaxSize = 20)]
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] Password;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public string Email;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Data2;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharName;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt4>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt4();
                        pParser((PacketSt4)message.PacketTemplate, message.PacketData);

                        var clientOld = World.GetClient(Idents.Clients.NetID, message.NetID);
                        
                        // refresh login counter
                        clientOld.LoginCount = (int)((PacketSt4) message.PacketTemplate).LoginCount;

                        // get the email
                        clientOld.Email = ((PacketSt4) message.PacketTemplate).Email;

                        // get the pw Note: dirty pw encryption workaround here...
                        string pw = "";
                        ((PacketSt4)message.PacketTemplate).Password[0] /= 2; // because UTF16 has 2 bytes per character
                        ((PacketSt4)message.PacketTemplate).Password[1] = 0; // because that was not set correctly by the client
                        RawConverter.ReadUTF16(ref pw, new MemoryStream(((PacketSt4) message.PacketTemplate).Password));
                        clientOld.Password = pw;

                        // check the login with database values
                        using (var db = (MySQL) DataBaseProvider.GetDataBase())
                        {
                                var dbClient = from acc in db.accountsMasterData
                                                where (acc.email == clientOld.Email
                                                        && acc.password == clientOld.Password)
                                                select acc;

                                // check for typical errors
                                var errorcode = 0;
                                if (dbClient.Count() == 0)
                                {
                                        errorcode = 227;
                                }
                                else if (dbClient.First().isBanned == 1)
                                {
                                        errorcode = 45;
                                }

                                // if an error occured
                                if (errorcode != 0)
                                {
                                        // send a stream terminator:
                                        var msg = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                        };
                                        // set the message data
                                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)clientOld.LoginCount;
                                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = (uint)errorcode;
                                        // send it
                                        QueuingService.PostProcessingQueue.Enqueue(msg);

                                        // reset the security data
                                        clientOld.Email = "";
                                        clientOld.Password = "";
                                }
                                // if login data is valid and user has not been banned
                                else
                                {
                                        var accountID = dbClient.First().accountID;
                                        var charID = dbClient.First().charID ?? 0;

                                        // Note: does this really generate different random numbers each time?
                                        var ran = new Random();
                                        ran.NextBytes(clientOld.SecurityKeys[0]);
                                        ran.NextBytes(clientOld.SecurityKeys[1]);

                                        clientOld.Status = SyncState.UpdateClientLogin;
                                                
                                        // Note IMPORTANT: RESET THE CLIENT WITHIN WORLD, CAUSE ACCID AND CHARID NEED TO BE ADDED!
                                        Client client = new Client(message.NetID, accountID, charID)
                                                                {
                                                                        Email = clientOld.Email,
                                                                        InitCryptSeed = clientOld.InitCryptSeed,
                                                                        LoginCount = clientOld.LoginCount,
                                                                        Password = clientOld.Password,
                                                                        SecurityKeys = clientOld.SecurityKeys,
                                                                        Status = SyncState.AtCharView,
                                                                };

                                        World.UpdateClient(clientOld, client);

                                        // send charinfo packets if necessary);
                                        if ((int)client[Idents.Clients.CharID] != 0)
                                        {
                                                var accID = (int)client[Idents.Clients.AccID];

                                                var dbChars = from c in db.charsMasterData
                                                                where c.accountID == accID
                                                                select c;

                                                foreach (var dbChar in dbChars)
                                                {
                                                        // update the client if this was the default selected char
                                                        if (dbChar.charID == ((int)client[Idents.Clients.CharID]))
                                                        {
                                                                client.MapID = dbChar.mapID;
                                                        }

                                                        // send a char info packet:
                                                        var charAppearance = new NetworkMessage(message.NetID)
                                                                                {
                                                                                        PacketTemplate =new P07_CharacterInfo.PacketSt7()
                                                                                };
                                                        // set the message data
                                                        ((P07_CharacterInfo.PacketSt7) charAppearance.PacketTemplate).LoginCount = (uint) client.LoginCount;
                                                        ((P07_CharacterInfo.PacketSt7) charAppearance.PacketTemplate).StaticHash1 = new byte[16];
                                                        ((P07_CharacterInfo.PacketSt7) charAppearance.PacketTemplate).StaticData1 = 0;
                                                        ((P07_CharacterInfo.PacketSt7) charAppearance.PacketTemplate).CharName = dbChar.charName;
                                                                
                                                        // create the appearance bytearray
                                                        #region appearance
                                                        byte remainderLen = 0;

                                                        if (dbChar.armorHead.Length != 0) remainderLen++;
                                                        if (dbChar.armorChest.Length != 0) remainderLen++;
                                                        if (dbChar.armorArms.Length != 0) remainderLen++;
                                                        if (dbChar.armorLegs.Length != 0) remainderLen++;
                                                        if (dbChar.armorFeet.Length != 0) remainderLen++;

                                                        var appearance = new MemoryStream();
                                                        RawConverter.WriteUInt16(6, appearance);
                                                        RawConverter.WriteUInt16((ushort)(from m in db.mapsMasterData where m.gameMapID == dbChar.mapID select m).First().gameMapID, appearance);
                                                        RawConverter.WriteByteAr(new byte[] {0x33, 0x36, 0x31, 0x30,}, appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookHeight << 4) | dbChar.lookSex), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookHairColor << 4) | dbChar.lookSkinColor), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.professionPrimary << 4) | dbChar.lookHairStyle), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookCampaign << 4) | dbChar.lookFace), appearance);
                                                        RawConverter.WriteByteAr(new byte[16], appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.level << 4) | dbChar.lookCampaign), appearance);
                                                        RawConverter.WriteByte((byte)
                                                                ((128) |
                                                                (dbChar.showHelm == 1? 64 : 0) |
                                                                (dbChar.professionSecondary << 2) |
                                                                (dbChar.isPvP == 1 ? 2 : 0) |
                                                                (dbChar.level > 15 ? 1 : 0)),
                                                                appearance);
                                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD }, appearance);
                                                        RawConverter.WriteByte(remainderLen, appearance);
                                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD, 0xDD, 0xDD }, appearance);
                                                        if (dbChar.armorHead != null) RawConverter.WriteByteAr(dbChar.armorHead, appearance);
                                                        if (dbChar.armorChest != null) RawConverter.WriteByteAr(dbChar.armorChest, appearance);
                                                        if (dbChar.armorArms != null) RawConverter.WriteByteAr(dbChar.armorArms, appearance);
                                                        if (dbChar.armorLegs != null) RawConverter.WriteByteAr(dbChar.armorLegs, appearance);
                                                        if (dbChar.armorFeet != null) RawConverter.WriteByteAr(dbChar.armorFeet, appearance);
                                                        #endregion appearance

                                                        ((P07_CharacterInfo.PacketSt7)charAppearance.PacketTemplate).ArraySize1 = (ushort)appearance.Length;
                                                        ((P07_CharacterInfo.PacketSt7)charAppearance.PacketTemplate).Appearance = appearance.ToArray();
                                                        // send it
                                                        QueuingService.PostProcessingQueue.Enqueue(charAppearance);
                                                }

                                                // send a account data packet:
                                                var accountData = new NetworkMessage(message.NetID)
                                                {
                                                        PacketTemplate =new P22_AccountGuiSettings.PacketSt22()
                                                };
                                                // add data
                                                ((P22_AccountGuiSettings.PacketSt22)accountData.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                                ((P22_AccountGuiSettings.PacketSt22)accountData.PacketTemplate).RawData = dbClient.First().guiSettings;
                                                ((P22_AccountGuiSettings.PacketSt22)accountData.PacketTemplate).ArraySize1 = (ushort)((P22_AccountGuiSettings.PacketSt22)accountData.PacketTemplate).RawData.Length;
                                                // send it
                                                QueuingService.PostProcessingQueue.Enqueue(accountData);

                                                // send a friendslist end packet:
                                                var friendsListEnd = new NetworkMessage(message.NetID)
                                                {
                                                        PacketTemplate = new P20_FriendsListEnd.PacketSt20()
                                                };
                                                // add data
                                                ((P20_FriendsListEnd.PacketSt20)friendsListEnd.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                                ((P20_FriendsListEnd.PacketSt20)friendsListEnd.PacketTemplate).StaticData1 = 1;
                                                // send it
                                                QueuingService.PostProcessingQueue.Enqueue(friendsListEnd);

                                                // send a acc permissions packet:
                                                var accountPermissions = new NetworkMessage(message.NetID)
                                                {
                                                        PacketTemplate = new P17_AccountPermissions.PacketSt17()
                                                };
                                                // add data
                                                ((P17_AccountPermissions.PacketSt17)accountPermissions.PacketTemplate).LoginCount = (uint)client.LoginCount;
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
                                                ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                                                ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = 0;
                                                // send it
                                                QueuingService.PostProcessingQueue.Enqueue(msg);
                                        }
                                }
                                
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt4> pParser;
        }
}
