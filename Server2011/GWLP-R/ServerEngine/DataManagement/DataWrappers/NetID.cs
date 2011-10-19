using System;

namespace ServerEngine.DataManagement.DataWrappers
{
        public class NetID : IWrapper<int>
        {
                public int Value { get; set; }

                public NetID(int value)
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