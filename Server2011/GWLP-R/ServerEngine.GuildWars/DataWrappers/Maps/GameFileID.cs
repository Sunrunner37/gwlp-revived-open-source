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

                public ulong Hash()
                {
                        return (((ulong)GetType().GetHashCode() << 32) & 0xFFFFFFFF00000000) |
                               ((ulong)Value.GetHashCode() & 0x00000000FFFFFFFF);
                }
        }
}