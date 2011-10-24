using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
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
                        Weaponsets = new Dictionary<int, Weaponset>();
                        Equipment = new Dictionary<AgentEquipment, Item>();
                }

                /// <summary>
                ///   Selects all dictionary key-value pairs that lie within a special item storage
                /// </summary>
                public CharacterItems Get(ItemStorage storageType)
                {
                        // the following linq expressions filters the dictionary and returns a new one,
                        // only containing items that are located at a specific storage
                        return (CharacterItems)this
                                .AsEnumerable()
                                .Where(x => x.Value.Data.Storage == storageType)
                                .ToDictionary(x => x.Key, x => x.Value);
                }

                /// <summary>
                ///   This property contains all available weapon sets of a character
                /// </summary>
                public Dictionary<int, Weaponset> Weaponsets { get; set; }

                /// <summary>
                ///   This property holds all equiped items
                /// </summary>
                public Dictionary<AgentEquipment, Item> Equipment { get; set; }

                /// <summary>
                ///   This function returns the first free slot in the characters bags.
                ///   Returns true if a free slot was found.
                /// </summary>
                public bool GetFirstFreeSlot(out ItemStorage storage, out byte slot)
                {
                        Item bag;

                        for (int i = (int)AgentEquipment.Backpack; i <= (int)AgentEquipment.EquipmentPack; i++)
                        {
                                if (!Equipment.TryGetValue((AgentEquipment) i, out bag)) continue;

                                var slots = bag.GetBagSize();

                                if (slots > 0) // failcheck
                                {
                                        storage = (ItemStorage)(i - (int)AgentEquipment.Backpack);
                                        CharacterItems itemsInBag = Get(storage);
                                        bool[] slotOccupied = new bool[slots];

                                        foreach (var item in itemsInBag)
                                        {
                                                slotOccupied[item.Value.Data.Slot] = true;
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

                        storage = ItemStorage.Backpack;
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