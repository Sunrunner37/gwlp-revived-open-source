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

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}