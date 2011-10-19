using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Chars
{
        public class LocalID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public LocalID(uint value)
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