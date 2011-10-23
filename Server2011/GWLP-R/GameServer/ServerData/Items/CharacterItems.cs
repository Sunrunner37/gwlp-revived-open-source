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
                        Equipment = new Dictionary<AgentEquipment, ItemBag>();
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
                public Dictionary<AgentEquipment, ItemBag> Equipment { get; set; }

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
                                        Leadhand = Equipment.ContainsKey(AgentEquipment.Leadhand) ? (uint)Equipment[AgentEquipment.Leadhand].Item.Data.ItemLocalID : 0,
                                        Offhand = Equipment.ContainsKey(AgentEquipment.Offhand) ? (uint)Equipment[AgentEquipment.Offhand].Item.Data.ItemLocalID : 0,
                                        Head = Equipment.ContainsKey(AgentEquipment.Head) ? (uint)Equipment[AgentEquipment.Head].Item.Data.ItemLocalID : 0,
                                        Chest = Equipment.ContainsKey(AgentEquipment.Chest) ? (uint)Equipment[AgentEquipment.Chest].Item.Data.ItemLocalID : 0,
                                        Arms = Equipment.ContainsKey(AgentEquipment.Arms) ? (uint)Equipment[AgentEquipment.Arms].Item.Data.ItemLocalID : 0,
                                        Legs = Equipment.ContainsKey(AgentEquipment.Legs) ? (uint)Equipment[AgentEquipment.Legs].Item.Data.ItemLocalID : 0,
                                        Feet = Equipment.ContainsKey(AgentEquipment.Feet) ? (uint)Equipment[AgentEquipment.Feet].Item.Data.ItemLocalID : 0,
                                        Costume = Equipment.ContainsKey(AgentEquipment.Costume) ? (uint)Equipment[AgentEquipment.Costume].Item.Data.ItemLocalID : 0,
                                        CostumeHead = Equipment.ContainsKey(AgentEquipment.CostumeHead) ? (uint)Equipment[AgentEquipment.CostumeHead].Item.Data.ItemLocalID : 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charEquipment);
                }

                /// <summary>
                ///   Adds an item, and automatically saves it to the db
                /// </summary>
                /// <param name="item"></param>
                public void AddSave(Item item)
                {
                        // add it
                        Add(item.Data.ItemLocalID, item);

                        // and call the database save stuff
                        item.SaveToDB();
                }

                public new bool Remove(int key)
                {
                        ItemBag bag;

                        // storage + backpack -> this is a conversion between ItemStorage and AgentEquipment
                        if (Equipment.TryGetValue((AgentEquipment)base[key].Data.Storage + (int)AgentEquipment.Backpack, out bag))
                        {
                                bag.GetFreeSlot();
                        }

                        return base.Remove(key);
                }

                /// <summary>
                ///   Fast lookup of a free item slot,
                ///   this is not garuanteed to be the first slot, and it may return a -1 even if there
                ///   is a free slot somewhere.
                ///   This is just to be used as a quick way to get a slot.
                /// </summary>
                /// <returns></returns>
                public int GetFreeSlot()
                {
                        ItemBag bag;

                        for (var i = 0; i < 4; i++)
                        {
                                // get the equipment-part (by the help of the enum)
                                // we only need the 4 values from backpack to bag2
                                if (Equipment.TryGetValue((AgentEquipment)i + (int)AgentEquipment.Backpack, out bag))
                                {
                                        return bag.GetFreeSlot();
                                }
                        }
 
                        return -1;
                }
        }
}