using System;
using System.Linq;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;
using GameServer.ServerData.Items;
using GameServer.Enums;
using System.Collections.Generic;
using GameServer.Packets.ToClient;
using ServerEngine;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 107)]
        public class P107_MoveItem : IPacket
        {
                public class PacketSt107 : IPacketTemplate
                {
                        public UInt16 Header { get { return 107; } }
                        public UInt32 ItemID;
                        public UInt16 PageID;
                        public byte Slot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt107>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt107();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        // item exists
                        Item itemToMove;
                        if (!chara.Data.Items.TryGetValue((int)pack.ItemID, out itemToMove)) return true; // item did not exist

                        ItemStorage storage = (ItemStorage) pack.PageID;
                        byte slot = pack.Slot;

                        if (slot == 0xFF) // item deequiped to bag
                        {
                                if (!chara.Data.Items.GetFirstFreeSlotInBag(storage, out slot))
                                {
                                        var equipBag2 = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P306_EquipBag2.PacketSt306
                                                {
                                                        StorageID = 11,
                                                        BagLocalID = 0
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(equipBag2);

                                        return true;
                                }
                        }

                        var itemsAtSlot = chara.Data.Items.Get(storage).Values.Where(curItem => curItem.Data.Slot == slot);

                        if (itemsAtSlot.Count() > 0)
                        {
                                var item = itemsAtSlot.First();

                                item.Data.Storage = itemToMove.Data.Storage;
                                item.Data.Slot = itemToMove.Data.Slot;
                                item.SaveToDB();
                                itemToMove.Data.Storage = storage;
                                itemToMove.Data.Slot = slot;
                                itemToMove.SaveToDB();

                                var swapItems = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P328_SwapItems.PacketSt328
                                        {
                                                ItemStreamID = 1,
                                                MovedItemLocalID = (uint)itemToMove.Data.ItemLocalID,
                                                ItemToBeSwappedWithLocalID = (uint)item.Data.ItemLocalID

                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(swapItems);

                                return true;
                        }

                        itemToMove.Data.Storage = storage;
                        itemToMove.Data.Slot = slot;
                        itemToMove.SaveToDB();
                        itemToMove.SendMove(chara.Data.NetID);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt107> pParser;
        }
}
