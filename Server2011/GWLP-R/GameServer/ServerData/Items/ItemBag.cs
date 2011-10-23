using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Enums;
using ServerEngine;

namespace GameServer.ServerData.Items
{
        public class ItemBag
        {
                private int maxSlots;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                /// <param name="item"></param>
                public ItemBag(Item item)
                {
                        maxSlots = item.GetBagSize();
                        slotIDs = new IDManager(0, maxSlots);
                }

                /// <summary>
                ///   This property holds the storage type of this storage
                /// </summary>
                public ItemStorage Type { get; set; }

                /// <summary>
                ///   This item hold the storage item itself
                /// </summary>
                public Item Item { get; set; }

                /// <summary>
                ///   Returns a free slot, -1 if this item has no free slot
                /// </summary>
                public int GetFreeSlot()
                {
                        try
                        {
                                return slotIDs.RequestID();
                        }
                        catch (IndexOutOfRangeException)
                        {

                                return -1;
                        }
                }

                /// <summary>
                ///   Recalculates the free item ID's
                /// </summary>
                /// <param name="items"></param>
                public void RecalculateSlotIDs(IEnumerable<Item> items)
                {
                        slotIDs = new IDManager(0, maxSlots);

                        foreach (var item in items.Where(item => item.Data.Storage == Type))
                        {
                                slotIDs.ClaimID(item.Data.Slot);
                        }
                }

                private IDManager slotIDs;
        }
}