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

                public ulong Hash()
                {
                        return (((ulong)GetType().GetHashCode() << 32) & 0xFFFFFFFF00000000) |
                               ((ulong)Value.GetHashCode() & 0x00000000FFFFFFFF);
                }
        }
}