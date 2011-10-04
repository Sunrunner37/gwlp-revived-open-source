using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Maps
{
        /// <summary>
        ///   This is the internally used MapID! It may be different to the MapID used by GW!
        /// </summary>
        public class MapID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public MapID(uint value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}