using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using LoginServer.ServerData.DataInterfaces;
using ServerEngine;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.DataManagement;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 4)]
        public class P04_AccountLogin : IPacket
        {
                public class PacketSt4 : IPacketTemplate, IHasAccountData
                {
                        public UInt16 Header { get { return 4; } }
                        public UInt32 LoginCount;
                        //public UInt32 PwSize;
                        //[PacketFieldType(ConstSize = true, MaxSize = 20)]
                        [PacketFieldType(ConstSize = true, MaxSize = 24)]
                        public byte[] ClPassword;
                        [PacketFieldType(ConstSize = false, MaxSize = 64)]
                        public string ClEmail;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string Data2;
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharName;

                        #region Implementation of IHasAccountData

                        public string Email
                        {
                                get { return ClEmail; }
                                set { ClEmail = value; }
                        }

                        public string Password
                        {
                                get
                                {
                                        var pw = "";
                                        var raw = (byte[])ClPassword.Clone();
                                        raw[0] /= 2; // because UTF16 has 2 bytes per character
                                        raw[1] = 0;  // because that was not set correctly by the client
                                        RawConverter.ReadUTF16(ref pw, new MemoryStream(raw));
                                        return pw;

                                }
                                set { ClPassword =new byte[0]; }
                        }

                        #endregion
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
                        var pack = new PacketSt4();
                        pParser(pack, message.PacketData);

                        var oldClient = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // refresh login counter
                        oldClient.Data.SyncCount = pack.LoginCount;

                        // get the account data
                        oldClient.Data.Paste<IHasAccountData>(pack);

                        // check the login with database values
                        using (var db = (MySQL) DataBaseProvider.GetDataBase())
                        {
                                var dbClient = from acc in db.accountsMasterData
                                                where (acc.email == oldClient.Data.Email
                                                        && acc.password == oldClient.Data.Password)
                                                select acc;

                                // check for typical errors
                                var errorcode = (uint)0;
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
                                        // Note: STREAM TERMINATOR
                                        var msg = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P03_StreamTerminator.PacketSt3
                                                {
                                                        LoginCount = oldClient.Data.SyncCount,
                                                        ErrorCode = errorcode
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(msg);

                                        // reset the security data
                                        oldClient.Data.Email = "";
                                        oldClient.Data.Password = "";
                                }
                                // if login data is valid and user has not been banned
                                else
                                {
                                        var accountID = (uint)dbClient.First().accountID;
                                        var charID = (uint)(dbClient.First().charID ?? 0);
                                        var mapID = 0;

                                        // get the map id...
                                        var chars = from c in db.charsMasterData
                                                      where c.charID == charID
                                                      select c;

                                        if (chars.Count() != 0) mapID = chars.First().mapID;


                                        // generate some random numbers
                                        var ran = new Random();
                                        ran.NextBytes(oldClient.Data.SecurityKeys[0]);
                                        ran.NextBytes(oldClient.Data.SecurityKeys[1]);

                                        oldClient.Data.Status = SyncStatus.UpdateClientLogin;
                                                
                                        //create a new client and add the new references
                                        var newClient = new DataClient(
                                                new ClientData
                                                {
                                                        AccID = new AccID(accountID),
                                                        CharID = new CharID(charID),
                                                        MapID = new MapID((uint)mapID)
                                                });

                                        // copy the old data
                                        newClient.Data.Paste<IHasNetworkData>(oldClient.Data);
                                        newClient.Data.Paste<IHasAccountData>(oldClient.Data);
                                        newClient.Data.Paste<IHasEncryptionData>(oldClient.Data);
                                        newClient.Data.Paste<IHasTransferSecurityData>(oldClient.Data);
                                        newClient.Data.Paste<IHasSyncStatusData>(oldClient.Data);

                                        // replace the old client with the new one
                                        if (!LoginServerWorld.Instance.Update(oldClient, newClient))
                                        {
                                                Debug.WriteLine("Update client failed at login!");
                                        }

                                        // update the client sync status
                                        newClient.Data.Status = SyncStatus.AtCharView;

                                        // send charinfo packets if necessary);
                                        if (newClient.Data.CharID.Value != 0)
                                        {
                                                // get the characters from the db
                                                var dbChars = from c in db.charsMasterData
                                                                where c.accountID == accountID
                                                                select c;

                                                foreach (var dbChar in dbChars)
                                                {
                                                        // create the appearance bytearray
                                                        #region appearance

                                                        byte remainderLen = 0;

                                                        /*if (dbChar.armorHead.Length != 0) remainderLen++;
                                                        if (dbChar.armorChest.Length != 0) remainderLen++;
                                                        if (dbChar.armorArms.Length != 0) remainderLen++;
                                                        if (dbChar.armorLegs.Length != 0) remainderLen++;
                                                        if (dbChar.armorFeet.Length != 0) remainderLen++;*/

                                                        var appearance = new MemoryStream();
                                                        RawConverter.WriteUInt16(6, appearance);
                                                        RawConverter.WriteUInt16((ushort)(from m in db.mapsMasterData where m.gameMapID == dbChar.mapID select m).First().gameMapID, appearance);
                                                        RawConverter.WriteByteAr(new byte[] { 0x33, 0x36, 0x31, 0x30, }, appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookHeight << 4) | dbChar.lookSex), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookHairColor << 4) | dbChar.lookSkinColor), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.professionPrimary << 4) | dbChar.lookHairStyle), appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.lookCampaign << 4) | dbChar.lookFace), appearance);
                                                        RawConverter.WriteByteAr(new byte[16], appearance);
                                                        RawConverter.WriteByte((byte)((dbChar.level << 4) | dbChar.lookCampaign), appearance);
                                                        RawConverter.WriteByte((byte)
                                                                ((128) |
                                                                (dbChar.showHelm == 1 ? 64 : 0) |
                                                                (dbChar.professionSecondary << 2) |
                                                                (dbChar.isPvP == 1 ? 2 : 0) |
                                                                (dbChar.level > 15 ? 1 : 0)),
                                                                appearance);
                                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD }, appearance);
                                                        RawConverter.WriteByte(remainderLen, appearance);
                                                        RawConverter.WriteByteAr(new byte[] { 0xDD, 0xDD, 0xDD, 0xDD }, appearance);
                                                        /*if (dbChar.armorHead != null) RawConverter.WriteByteAr(dbChar.armorHead, appearance);
                                                        if (dbChar.armorChest != null) RawConverter.WriteByteAr(dbChar.armorChest, appearance);
                                                        if (dbChar.armorArms != null) RawConverter.WriteByteAr(dbChar.armorArms, appearance);
                                                        if (dbChar.armorLegs != null) RawConverter.WriteByteAr(dbChar.armorLegs, appearance);
                                                        if (dbChar.armorFeet != null) RawConverter.WriteByteAr(dbChar.armorFeet, appearance);*/

                                                        #endregion appearance

                                                        // Note: CHAR INFO PACKET
                                                        var charAppearance = new NetworkMessage(message.NetID)
                                                        {
                                                                PacketTemplate = new P07_CharacterInfo.PacketSt7
                                                                {
                                                                        // set the message data
                                                                        LoginCount = newClient.Data.SyncCount,
                                                                        StaticHash1 = new byte[16],
                                                                        StaticData1 = 0,
                                                                        CharName = dbChar.charName,
                                                                        ArraySize1 = (ushort)appearance.Length,
                                                                        Appearance = appearance.ToArray(),
                                                                }
                                                        };
                                                        QueuingService.PostProcessingQueue.Enqueue(charAppearance);
                                                }
                                        }

                                        // Note: ACCOUNT GUI SETTINGS
                                        var accountData = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P22_AccountGuiSettings.PacketSt22
                                                {
                                                        LoginCount = newClient.Data.SyncCount,
                                                        RawData = dbClient.First().guiSettings,
                                                        ArraySize1 = (ushort)dbClient.First().guiSettings.Length                     
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(accountData);

                                        // Note: FRIENDSLIST END
                                        var friendsListEnd = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P20_FriendsListEnd.PacketSt20
                                                {
                                                        LoginCount = newClient.Data.SyncCount,
                                                        StaticData1 = 1
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(friendsListEnd);

                                        // Note: ACCOUNT PERMISSIONS
                                        var accountPermissions = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P17_AccountPermissions.PacketSt17
                                                {
                                                        LoginCount = newClient.Data.SyncCount,
                                                        Territory = 2, // sd america
                                                        TerritoryChanges = 4,
                                                        Data1 = new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                                                        Data2 = new byte[] { 0x80, 0x3F, 0x02, 0x00, 0x03, 0x00, 0x08, 0x00 },
                                                        Data3 = new byte[] { 0x37, 0x4B, 0x09, 0xBB, 0xC2, 0xF6, 0x74, 0x43, 0xAA, 0xAB, 0x35, 0x4D, 0xEE, 0xB7, 0xAF, 0x08 },
                                                        Data4 = new byte[] { 0x55, 0xB6, 0x77, 0x59, 0x0C, 0x0C, 0x15, 0x46, 0xAD, 0xAA, 0x33, 0x43, 0x4A, 0x91, 0x23, 0x6A },
                                                        ChangeAccSettings = 8,
                                                        ArraySize1 = 8,
                                                        AddedKeys = new byte[] { 0x01, 0x00, 0x06, 0x00, 0x57, 0x00, 0x01, 0x00 },
                                                        EulaAccepted = 23,
                                                        Data5 = 0,     
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(accountPermissions);

                                        // Note: STREAM TERMINATOR
                                        var msg = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P03_StreamTerminator.PacketSt3
                                                {
                                                        LoginCount = newClient.Data.SyncCount,
                                                        ErrorCode = 0
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(msg);
                                }
                                
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt4> pParser;
        }
}
