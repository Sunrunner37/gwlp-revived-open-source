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

                public int Hash()
                {
                        return (BitConverter.ToString(Value).GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}