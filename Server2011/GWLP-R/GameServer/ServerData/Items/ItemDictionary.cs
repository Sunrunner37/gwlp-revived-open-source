using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;

namespace GameServer.ServerData.Items
{
        public class ItemDictionary : Dictionary<int, Item>
        {
                /// <summary>
                ///   Selects all dictionary key-value pairs that lie within a special item storage
                /// </summary>
                public Dictionary<int, Item> Get(ItemStorage storageType)
                {
                        // the following linq expressions filters the dictionary and returns a new one,
                        // only containing items that are located at a specific storage
                        return this
                                .AsEnumerable()
                                .Where(x => x.Value.Data.Storage == storageType)
                                .ToDictionary(x => x.Key, x => x.Value);
                }
        }
}