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

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}