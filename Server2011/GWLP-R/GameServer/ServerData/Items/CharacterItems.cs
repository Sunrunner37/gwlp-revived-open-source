using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.NetworkManagement;

namespace GameServer.ServerData.Items
{
        public class CharacterItems : Dictionary<int, Item>
        {
                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public CharacterItems()
                {
                        var dummy = new Item();

                        Weaponsets = new Dictionary<int, Weaponset>()
                        {
                                {0, new Weaponset {LeadHand = dummy, OffHand = dummy, Number = 0}},
                                {1, new Weaponset {LeadHand = dummy, OffHand = dummy, Number = 1}},
                                {2, new Weaponset {LeadHand = dummy, OffHand = dummy, Number = 2}},
                                {3, new Weaponset {LeadHand = dummy, OffHand = dummy, Number = 3}}
                        };
                        Equipment = new Dictionary<AgentEquipment, Item>();
                }

                /// <summary>
                ///   Selects all dictionary key-value pairs that lie within a special item storage
                /// </summary>
                public Dictionary<int, Item> Get(ItemStorage storageType)
                {
                        // the following linq expressions filters the dictionary and returns a new one,
                        // only containing items that are located at a specific storage
                        return this.AsEnumerable()
                                .Where(x => x.Value.Data.Storage == storageType)
                                .ToDictionary(x => x.Key, x => x.Value);
                }

                /// <summary>
                ///   This property contains all available weapon sets of a character
                /// </summary>
                public Dictionary<int, Weaponset> Weaponsets { get; set; }

                /// <summary>
                ///   This property contains the active weaponset
                /// </summary>
                public Weaponset ActiveWeaponset { get; set; }

                /// <summary>
                ///   This property holds all equiped items
                /// </summary>
                public Dictionary<AgentEquipment, Item> Equipment { get; set; }

                /// <summary>
                ///   This function returns the first free slot in the characters bags.
                ///   Returns true if a free slot was found.
                /// </summary>
                public bool GetFirstFreeSlot(out ItemStorage bag, out byte slot)
                {
                        for (bag = ItemStorage.Backpack; bag <= ItemStorage.EquipmentPack; bag++)
                        {
                                if (GetFirstFreeSlotInBag(bag, out slot)) return true;
                        }

                        bag = ItemStorage.Backpack;
                        slot = 0;
                        return false;
                }

                /// <summary>
                ///   This function returns the number of free slots in the characters bags.
                /// </summary>
                public int GetNumFreeSlots()
                {
                        Item bag;
                        int numFreeSlots = 0;

                        for (int i = (int)ItemStorage.Backpack; i <= (int)ItemStorage.EquipmentPack; i++)
                        {
                                numFreeSlots += GetNumFreeSlotsInBag((ItemStorage)i);
                        }

                        return numFreeSlots;
                }

                /// <summary>
                ///   This function returns the number of free slots in a specific bag.
                /// </summary>
                public int GetNumFreeSlotsInBag(ItemStorage bag)
                {
                        Item bagItem;
                        if (Equipment.TryGetValue((AgentEquipment)(bag + (int)AgentEquipment.Backpack), out bagItem))
                        {
                                var slots = bagItem.GetBagSize();

                                if (slots > 0) // failcheck
                                {
                                        return slots - Get(bag).Count;
                                }
                        }

                        return 0;
                }

                /// <summary>
                ///   This function returns the first free slot in a specific bag.
                ///   Returns true if a free slot was found.
                /// </summary>
                public bool GetFirstFreeSlotInBag(ItemStorage bag, out byte slot)
                {
                        Item bagItem;
                        if (Equipment.TryGetValue((AgentEquipment)(bag + (int)AgentEquipment.Backpack), out bagItem))
                        {
                                var slots = bagItem.GetBagSize();

                                if (slots > 0) // failcheck
                                {
                                        var items = Get(bag);
                                        bool[] slotOccupied = new bool[slots];

                                        foreach (var item in items.Values)
                                        {
                                                slotOccupied[item.Data.Slot] = true;
                                        }

                                        for (slot = 0; slot < slots; slot++)
                                        {
                                                if (!slotOccupied[slot])
                                                {
                                                        return true;
                                                }
                                        }
                                }
                        }

                        slot = 0;
                        return false;
                }

                /// <summary>
                ///   Send the character's equipment packet
                /// </summary>
                /// <param name="netID"></param>
                public void SendEquipment(NetID netID, AgentID agentID)
                {
                        var charEquipment = new NetworkMessage(netID)
                        {
                                PacketTemplate = new P098_UpdateAgentFullEquipment.PacketSt98
                                {
                                        AgentID = agentID.Value,
                                        Leadhand = Equipment.ContainsKey(AgentEquipment.Leadhand) ? (uint)Equipment[AgentEquipment.Leadhand].Data.ItemLocalID : 0,
                                        Offhand = Equipment.ContainsKey(AgentEquipment.Offhand) ? (uint)Equipment[AgentEquipment.Offhand].Data.ItemLocalID : 0,
                                        Head = Equipment.ContainsKey(AgentEquipment.Head) ? (uint)Equipment[AgentEquipment.Head].Data.ItemLocalID : 0,
                                        Chest = Equipment.ContainsKey(AgentEquipment.Chest) ? (uint)Equipment[AgentEquipment.Chest].Data.ItemLocalID : 0,
                                        Arms = Equipment.ContainsKey(AgentEquipment.Arms) ? (uint)Equipment[AgentEquipment.Arms].Data.ItemLocalID : 0,
                                        Legs = Equipment.ContainsKey(AgentEquipment.Legs) ? (uint)Equipment[AgentEquipment.Legs].Data.ItemLocalID : 0,
                                        Feet = Equipment.ContainsKey(AgentEquipment.Feet) ? (uint)Equipment[AgentEquipment.Feet].Data.ItemLocalID : 0,
                                        Costume = Equipment.ContainsKey(AgentEquipment.Costume) ? (uint)Equipment[AgentEquipment.Costume].Data.ItemLocalID : 0,
                                        CostumeHead = Equipment.ContainsKey(AgentEquipment.CostumeHead) ? (uint)Equipment[AgentEquipment.CostumeHead].Data.ItemLocalID : 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charEquipment);
                }

                /// <summary>
                ///   Adds an item, and automatically saves it to the db
                /// </summary>
                public void AddSave(int itemLocalID, Item item)
                {
                        // add it
                        Add(itemLocalID, item);

                        // and call the database save stuff
                        item.SaveToDB();
                }
        }
}