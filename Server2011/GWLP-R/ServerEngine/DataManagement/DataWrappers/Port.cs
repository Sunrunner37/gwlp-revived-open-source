using System;

namespace ServerEngine.DataManagement.DataWrappers
{
        public class Port : IWrapper<uint>
        {
                public uint Value { get; set; }

                public Port(uint value)
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