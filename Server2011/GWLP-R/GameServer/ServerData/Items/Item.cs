using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameServer.DataBase;
using GameServer.Enums;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.Tools;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.NetworkManagement;
using GameServer.Packets.ToClient;

namespace GameServer.ServerData.Items
{
        public class Item
        {
                private object objLock = new object();
                private ItemData data;

                /// <summary>
                ///   Creates a new instance of the class.
                /// </summary>
                public Item()
                {
                        lock (objLock)
                        data = new ItemData();
                }

                /// <summary>
                ///   This property holds all the data of the item.
                ///   This also provides thread-safe access.
                /// </summary>
                public ItemData Data
                {
                        get { lock (objLock) return data; }
                        set { lock (objLock) data = value; }
                }

                /// <summary>
                ///   This generates the item packets and automatically sends them
                ///   to the specified client
                /// </summary>
                public void SendPackets(NetID netID)
                {
                        // send all packets from this item to 'netID'
                        var itemPacket = new NetworkMessage(netID)
                        {
                                PacketTemplate = new P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = (uint)Data.ItemLocalID,
                                        FileID = (uint)Data.GameItemFileID,
                                        ItemType = (byte)Data.Type,
                                        Data2 = 32,
                                        DyeColor = (ushort)Data.DyeColor,
                                        Data4 = 0,
                                        CanBeDyed = 1,
                                        Flags = (uint)Data.Flags,
                                        ItemID = (uint)Data.GameItemID,
                                        Quantity = (uint)Data.Quantity,
                                        NameHash = GWStringExtensions.ToGW(Data.Name),
                                        NumStats = (byte)Data.Stats.Count,
                                        Stats = StatsToGW(Data.Stats)
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(itemPacket);

                        // if its customized...
                        if (Data.CreatorName != null) //failcheck
                        {
                                if (Data.CreatorName.Length > 0)
                                {
                                        var itemOwner = new NetworkMessage(netID)
                                        {
                                                PacketTemplate = new P304_ItemOwnerName.PacketSt304
                                                {
                                                        ItemLocalID = (uint)Data.ItemLocalID,
                                                        CharName = Data.CreatorName
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(itemOwner);
                                }
                        }

                        // some items have unnecessary restrictments...
                        if (Data.Profession > 0)
                        {
                                var itemProfession = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P336_ItemProfession.PacketSt336
                                        {
                                                ItemLocalID = (uint)Data.ItemLocalID,
                                                Profession = Data.Profession
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(itemProfession);
                        }

                        // if the item is owned by someone send its inventory location
                        if (Data.OwnerAccID.Value > 0)
                        {
                                var bagSize = GetBagSize();

                                if (Data.Storage == ItemStorage.Equiped && bagSize > 0) // special handling for bags
                                {
                                        var itemPage = new NetworkMessage(netID)
                                        {
                                                PacketTemplate = new P309_ItemPagePacket.PacketSt309
                                                {
                                                        ItemStreamID = 1,
                                                        StorageType = 1, // must be a bag (only item to come here)
                                                        StorageID = (byte)(Data.Slot - AgentEquipment.Backpack),
                                                        PageID = (ushort)(Data.Slot - AgentEquipment.Backpack),
                                                        Slots = bagSize,
                                                        ItemLocalID = (uint)Data.ItemLocalID
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(itemPage);
                                }
                                else
                                {
                                        var itemLocation = new NetworkMessage(netID)
                                        {
                                                PacketTemplate = new P308_ItemLocation.PacketSt308
                                                {
                                                        ItemStreamID = 1,
                                                        ItemLocalID = (uint)Data.ItemLocalID,
                                                        PageID = (ushort)Data.Storage,
                                                        UserSlot = (byte)Data.Slot
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(itemLocation);
                                }
                        }
                }

                private static UInt32[] StatsToGW(List<ItemStat> stats)
                {
                        UInt32[] GWFormat = new UInt32[stats.Count];
                        int i = 0;

                        foreach (ItemStat stat in stats)
                        {
                                GWFormat[i++] = stat.ToGW();
                        }

                        return GWFormat;
                }

                private byte GetBagSize()
                {
                        ItemStat firstStat = data.Stats.First<ItemStat>();

                        if (firstStat.Stat == ItemStatEnums.Slots)
                        {
                                return firstStat.Value1;
                        }
                        else
                        {
                                return 0;
                        }
                }

                /// <summary>
                ///   Generates a new item object, containing the basic item data copied from the MasterData table
                ///   Note that this item has now owner (accID, charID)! do NOT save this to database as is!
                /// </summary>
                public static Item CreateItemStubFromDB(int dbItemID, int itemLocalID)
                {
                        // get the database stuff
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // get the master data
                                var masterDatas = from im in db.itemsMasterData
                                                  where im.itemID == dbItemID
                                                  select im;

                                // failcheck
                                if (masterDatas.Count() == 0) return null;
                                var masterData = masterDatas.First();

                                // create the item
                                var tmpItem = new Item
                                {
                                        Data = new ItemData
                                        {
                                                ItemLocalID = itemLocalID,
                                                ItemID = dbItemID,
                                                GameItemID = masterData.gameItemID,
                                                GameItemFileID = masterData.gameItemFileID,
                                                Name = masterData.name,
                                                Type = (ItemType)Enum.ToObject(typeof(ItemType), masterData.itemType),
                                        }
                                };

                                return tmpItem;
                        }
                }

                /// <summary>
                ///   Fill an item with values from the database, and set a new itemLocalID
                ///   Note that CharID will be set to 0 if it is 0 in the db!
                /// </summary>
                public static Item LoadFromDB(itemsPerSonALData personalDataBaseItem, int itemLocalID)
                {
                        // get the database stuff
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // make things easier for dblinq ;D
                                var tmpID = personalDataBaseItem.itemID;

                                // get the master data
                                var masterDatas = from im in db.itemsMasterData
                                                  where im.itemID == tmpID
                                                  select im;

                                // failcheck
                                if (masterDatas.Count() == 0) return null;
                                var masterData = masterDatas.First();

                                // create the item
                                var tmpItem = new Item
                                {
                                        Data = new ItemData
                                        {
                                                ItemLocalID = itemLocalID,
                                                ItemID = personalDataBaseItem.itemID,
                                                GameItemID = masterData.gameItemID,
                                                GameItemFileID = masterData.gameItemFileID,
                                                PersonalItemID = personalDataBaseItem.personalItemID,
                                                OwnerAccID = new AccID((uint)personalDataBaseItem.accountID),
                                                OwnerCharID = new CharID((uint)personalDataBaseItem.charID),
                                                Name = masterData.name,
                                                Type = (ItemType)Enum.ToObject(typeof(ItemType), masterData.itemType),
                                                DyeColor = personalDataBaseItem.dyeColor,
                                                Flags = personalDataBaseItem.flags,
                                                Quantity = personalDataBaseItem.quantity,
                                                Storage = (ItemStorage)Enum.ToObject(typeof(ItemStorage), personalDataBaseItem.storage),
                                                Slot = personalDataBaseItem.slot,
                                                CreatorCharID = personalDataBaseItem.creatorCharID,
                                                CreatorName = personalDataBaseItem.creatorName,
                                                Profession = masterData.profession
                                        }
                                };

                                // add the stats
                                var ms = new MemoryStream(personalDataBaseItem.stats);
                                for (var i = 0; i < (personalDataBaseItem.stats.Length/4); i++)
                                {
                                        // read data
                                        var buf = new byte[4];
                                        RawConverter.ReadByteAr(ref buf, ms, 4);
                                        
                                        // create & add the stat
                                        tmpItem.Data.Stats.Add(new ItemStat(buf));
                                }

                                // finally, return the generated item
                                return tmpItem;
                        }
                }

                /// <summary>
                ///   Update / Add this item to the db.
                /// </summary>
                public void SaveToDB()
                {
                        // get the database stuff
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // get the db item
                                var items = from im in db.itemsPerSonALData
                                            where im.personalItemID == data.PersonalItemID
                                            select im;

                                var existsAlready = false;
                                var item = new itemsPerSonALData();

                                // check if it exists
                                if (items.Count() != 0)
                                {
                                        existsAlready = true;
                                        item = items.First();
                                }

                                // determine the char id (0 if the item lies in storage)
                                var charID = ((int)Data.Storage >= 5 && (int)Data.Storage <= 14) ? 0 : Data.OwnerCharID.Value;

                                // update the item
                                item.personalItemID = data.PersonalItemID;
                                item.itemID = Data.ItemID;
                                item.accountID = (int)Data.OwnerAccID.Value;
                                item.charID = (int)charID;
                                item.dyeColor = Data.DyeColor;
                                item.flags = Data.Flags;
                                item.quantity = Data.Quantity;
                                item.storage = (int)Data.Storage;
                                item.slot = Data.Slot;
                                item.creatorCharID = Data.CreatorCharID;
                                item.creatorName = Data.CreatorName;

                                // change the stats
                                var tmpList = new List<byte>();
                                foreach (var stat in Data.Stats.Select(x => x.Compile()))
                                {
                                        tmpList.AddRange(stat);
                                }
                                item.stats = tmpList.ToArray();

                                // finally, change the database
                                if (!existsAlready) db.itemsPerSonALData.InsertOnSubmit(item);

                                // submit changes, inserting the item (if necessary) or updating the old one
                                db.SubmitChanges();
                        }
                }
        }

        public class ItemData
        {
                /// <summary>
                ///   Creates a new instance of the class.
                /// </summary>
                public ItemData()
                {
                        OwnerAccID = new AccID(0);
                        OwnerCharID = new CharID(0);

                        Stats = new List<ItemStat>();
                }

                /// <summary>
                ///   The so called 'Item-Glue', is a generated local ID for the loaded Items
                /// </summary>
                public int ItemLocalID { get; set; }

                /// <summary>
                ///   This ID is only used in the database 'items_masterdata'
                /// </summary>
                public int ItemID { get; set; }

                /// <summary>
                ///   The gw-internal ItemID
                /// </summary>
                public int GameItemID { get; set; }

                /// <summary>
                ///   The file id (also called file-hash within the gw.dat-explorers)
                ///   of the item-3d-model file
                /// </summary>
                public uint GameItemFileID { get; set; }

                /// <summary>
                ///   This id is only used within the database 'items_personaldata' and
                ///   'items_personalstats'
                /// </summary>
                public long PersonalItemID { get; set; }

                /// <summary>
                ///   The account id of the owner
                /// </summary>
                public AccID OwnerAccID { get; set; }

                /// <summary>
                ///   The character id of the owner (THIS MAY BE '0'!)
                /// </summary>
                public CharID OwnerCharID { get; set; }

                /// <summary>
                ///   The name of the item, without 'mods'
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                ///   The type of the item, see the enum
                /// </summary>
                public ItemType Type { get; set; }

                /// <summary>
                ///   The profession of the item to make it limited to one profession
                /// </summary>
                public byte Profession { get; set; }

                /// <summary>
                ///   The color of the item (if it is dyed)
                /// </summary>
                public int DyeColor { get; set; }

                /// <summary>
                ///   These flags have effects on the item's visual stuff
                /// </summary>
                public int Flags { get; set; }

                /// <summary>
                ///   Determines the quantity of the item-stack (1 = only one item :P)
                /// </summary>
                public int Quantity { get; set; }

                /// <summary>
                ///   Determines the storage of the item.
                /// </summary>
                public ItemStorage Storage { get; set; }

                /// <summary>
                ///   Determines the slot of the storage the item lies in.
                /// </summary>
                public int Slot { get; set; }

                /// <summary>
                ///   Determines the char id of the char the item is bound to.
                ///   Note that this char can also be already deleted.
                /// </summary>
                public int CreatorCharID { get; set; }

                /// <summary>
                ///   The name of the char the item is bound to.
                /// </summary>
                public string CreatorName { get; set; }

                /// <summary>
                ///   This property holds all item stats.
                /// </summary>
                public List<ItemStat> Stats { get; set; }
        }
}