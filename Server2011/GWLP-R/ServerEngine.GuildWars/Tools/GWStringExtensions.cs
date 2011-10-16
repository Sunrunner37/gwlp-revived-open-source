using System;
using System.Text;

namespace ServerEngine.GuildWars.Tools
{
        public static class GWStringExtensions
        {
                public static string ToGW(this string text)
                {
                        var tmp = new StringBuilder();
                        tmp.Append(BitConverter.ToChar(new byte[] {0x08, 0x01}, 0));
                        tmp.Append(BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0));
                        tmp.Append(text);
                        tmp.Append(BitConverter.ToChar(new byte[] {0x01, 0x00}, 0));

                        return tmp.ToString();
                }
        }
}