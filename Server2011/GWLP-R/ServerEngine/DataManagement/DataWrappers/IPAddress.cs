using System;

namespace ServerEngine.DataManagement.DataWrappers
{
        public class IPAddress : IWrapper<byte[]>
        {
                public byte[] Value { get; set; }

                public IPAddress(byte[] value)
                {
                        Value = value;
                }

                public ulong Hash()
                {
                        return (((ulong)GetType().GetHashCode() << 32) & 0xFFFFFFFF00000000) |
                               ((ulong)BitConverter.ToInt32(Value, 0).GetHashCode() & 0x00000000FFFFFFFF);
                }
        }
}