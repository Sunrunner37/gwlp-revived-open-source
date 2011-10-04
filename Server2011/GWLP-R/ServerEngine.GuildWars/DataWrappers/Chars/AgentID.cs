using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Chars
{
        public class AgentID : IWrapper<uint>
        {
                public uint Value { get; set; }

                public AgentID(uint value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.ToString().GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}
