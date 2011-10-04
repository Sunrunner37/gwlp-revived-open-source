using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Maps
{
        /// <summary>
        ///   This is also known as the ZoneID, the GW-equivalent to MapID
        /// </summary>
        public class GameMapID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public GameMapID(uint value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}