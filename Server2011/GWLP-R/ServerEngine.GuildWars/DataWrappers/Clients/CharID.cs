using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Clients
{
        public class CharID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public CharID(uint value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}