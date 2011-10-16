using System;

namespace ServerEngine.GuildWars.Tools
{
        public static class GWStringExtensions
        {
                public static string ToGW(this string text)
                {
                        return
                                BitConverter.ToChar(new byte[] {0x08, 0x01}, 0) +
                                BitConverter.ToChar(new byte[] {0x07, 0x01}, 0) +
                                "GWLP" +
                                BitConverter.ToChar(new byte[] {0x01, 0x00}, 0);
                }
        }
}