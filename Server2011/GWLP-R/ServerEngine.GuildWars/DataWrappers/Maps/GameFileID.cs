using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Maps
{
        /// <summary>
        ///   This is also known as MapHash or MapFileHash or FileHash or FileID meaning the GW-internal 'file-name' of the
        ///   map file in the GW.dat
        /// </summary>
        public class GameFileID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public GameFileID(uint value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}