using System;
using System.Linq;
using LoginServer.DataBase;
using LoginServer.Enums;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using LoginServer.ServerData.DataInterfaces;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 10)]
        public class P10_CharacterPlayName : IPacket
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
                        var pack = new PacketSt10();
                        pParser(pack, message.PacketData);

                        // get the client
                        var oldClient = LoginServerWorld.Instance.Get<DataClient>(message.NetID);
                        
                        // update the sync counter
                        oldClient.Data.SyncCount = pack.LoginCount;

                        // Get the charID of a char with that name
                        uint charID, mapID;
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var name = pack.Name;
                                var accID = oldClient.Data.AccID.Value;

                                var chars = from c in db.charsMasterData
                                                where c.charName == name && c.accountID == accID
                                                select c;

                                if (chars.Count() == 0)
                                {
                                        // the client has send strange data, so kick it
                                        LoginServerWorld.Instance.Kick(oldClient);

                                        // and return true... the packet has been sucessfully handled
                                        return true;
                                }

                                var chara = chars.First();

                                charID = (uint)chara.charID;
                                mapID = (uint)chara.mapID;
                        }

                        // update the client with new charID
                        //create a new client and add the new references
                        var newClient = new DataClient(
                                new ClientData
                                {
                                        AccID = oldClient.Data.AccID,
                                        CharID = new CharID(charID),
                                        MapID = new MapID(mapID),
                                });

                        // copy the old data
                        newClient.Data.Paste<IHasNetworkData>(oldClient.Data);
                        newClient.Data.Paste<IHasAccountData>(oldClient.Data);
                        newClient.Data.Paste<IHasEncryptionData>(oldClient.Data);
                        newClient.Data.Paste<IHasTransferSecurityData>(oldClient.Data);
                        newClient.Data.Paste<IHasSyncStatusData>(oldClient.Data);

                        // replace the old client with the new one
                        LoginServerWorld.Instance.Update(oldClient, newClient);

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
                                PacketTemplate = new P03_StreamTerminator.PacketSt3()
                                {
                                        LoginCount = newClient.Data.SyncCount,
                                        ErrorCode = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(msg);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt10> pParser;
        }
}
