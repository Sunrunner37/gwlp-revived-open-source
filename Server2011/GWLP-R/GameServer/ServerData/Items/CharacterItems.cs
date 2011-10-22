using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;

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

                public Dictionary<AgentEquipment, Item> Equipment { get; set; }
        }
}