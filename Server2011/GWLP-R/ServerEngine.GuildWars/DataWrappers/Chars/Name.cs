using System;
using ServerEngine.DataManagement;

namespace ServerEngine.GuildWars.DataWrappers.Chars
{
        public class Name : IWrapper<string>
        {
                public string Value { get; set; }

                public Name(string value)
                {
                        Value = value;
                }

                public int Hash()
                {
                        return (Value.GetHashCode() << 16) | GetType().GetHashCode();
                }
        }
}