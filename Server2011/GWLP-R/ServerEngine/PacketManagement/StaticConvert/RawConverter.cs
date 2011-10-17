using System;
using System.IO;
using System.Text;

namespace ServerEngine.PacketManagement.StaticConvert
{
        /// <summary>
        ///   Note that this class is GuildWars-protocol dependant
        /// </summary>
        public static class RawConverter
        {
                public static void ReadByte(ref byte toData, MemoryStream fromRaw)
                {
                        toData = (byte)fromRaw.ReadByte();
                }

                public static void ReadUInt16(ref UInt16 toData, MemoryStream fromRaw)
                {
                        var buffer = new byte[2];
                        fromRaw.Read(buffer, 0, 2);

                        toData = buffer[0];
                        toData |= (UInt16)(buffer[1] << 8);
                }

                public static void ReadUInt32(ref UInt32 toData, MemoryStream fromRaw)
                {
                        var buffer = new byte[4];
                        fromRaw.Read(buffer, 0, 4);

                        toData = buffer[0];
                        toData |= (UInt32)(buffer[1] << 8);
                        toData |= (UInt32)(buffer[2] << 16);
                        toData |= (UInt32)(buffer[3] << 24);
                }

                public static void ReadFloat(ref float toData, MemoryStream fromRaw)
                {
                        var buffer = new byte[4];
                        fromRaw.Read(buffer, 0, 4);

                        toData = BitConverter.ToSingle(buffer, 0);
                }

                public static void ReadUTF16(ref string toData, MemoryStream fromRaw)
                {
                        UInt16 length = 0;

                        ReadUInt16(ref length, fromRaw);

                        // failcheck
                        if (length-1 > 0)
                        {
                                var buffer = new byte[length * 2]; // remember, UTF16 has WORD for each character
                                fromRaw.Read(buffer, 0, length * 2);

                                toData = Encoding.Unicode.GetString(buffer);
                        }
                }

                public static void ReadByteAr(ref byte[] toData, MemoryStream fromRaw, int byteLength)
                {

                        var buffer = new byte[byteLength];
                        fromRaw.Read(buffer, 0, byteLength);

                        toData = buffer;
                }

                public static void ReadUInt16Ar(ref UInt16[] toData, MemoryStream fromRaw, int byteLength)
                {
                        // check for correct element sizes
                        if ((byteLength % 2) == 0)
                        {
                                // enlarge the array
                                toData = new UInt16[byteLength / 2];

                                // and fill it up
                                for (int i = 0; i < toData.Length; i++)
                                {
                                        ReadUInt16(ref toData[i], fromRaw);
                                }
                        }
                }

                public static void ReadUInt32Ar(ref UInt32[] toData, MemoryStream fromRaw, int byteLength)
                {

                        // check for correct element sizes
                        if ((byteLength % 4) == 0)
                        {
                                // enlarge the array
                                toData = new UInt32[byteLength / 4];

                                // and fill it up
                                for (int i = 0; i < toData.Length; i++)
                                {
                                        ReadUInt32(ref toData[i], fromRaw);
                                }
                        }
                }

                public static void WriteByte(byte fromData, MemoryStream toRaw)
                {
                        toRaw.WriteByte(fromData);
                }

                public static void WriteUInt16(UInt16 fromData, MemoryStream toRaw)
                {
                        var buffer = new byte[2];
                        buffer[0] = (byte)(fromData & 0xFF);
                        buffer[1] = (byte)(fromData >> 8 & 0xFF);
                        toRaw.Write(buffer, 0, 2);
                }

                public static void WriteUInt32(UInt32 fromData, MemoryStream toRaw)
                {
                        var buffer = new byte[4];
                        buffer[0] = (byte)(fromData & 0xFF);
                        buffer[1] = (byte)(fromData >> 8 & 0xFF);
                        buffer[2] = (byte)(fromData >> 16 & 0xFF);
                        buffer[3] = (byte)(fromData >> 24 & 0xFF);
                        toRaw.Write(buffer, 0, 4);
                }

                public static void WriteFloat(float fromData, MemoryStream toRaw)
                {
                        var buffer = BitConverter.GetBytes(fromData);
                        toRaw.Write(buffer, 0, 4);
                }

                public static void WriteUTF16(string fromData, MemoryStream toRaw)
                {
                        // write length
                        WriteUInt16((UInt16)(fromData.Length), toRaw);

                        // write string (possible BUG: check if encoding works the right way)
                        var buffer = Encoding.Unicode.GetBytes(fromData);
                        toRaw.Write(buffer, 0, fromData.Length * 2);
                }

                public static void WriteByteAr(byte[] fromData, MemoryStream toRaw)
                {
                        if (fromData != null && fromData.Length > 0)
                        {
                                toRaw.Write(fromData, 0, fromData.Length);
                        }
                }

                public static void WriteUInt16Ar(UInt16[] fromData, MemoryStream toRaw)
                {
                        if (fromData.Length > 0)
                        {
                                foreach (var t in fromData)
                                {
                                        WriteUInt16(t, toRaw);
                                }
                        }
                }

                public static void WriteUInt32Ar(UInt32[] fromData, MemoryStream toRaw)
                {
                        if (fromData.Length > 0)
                        {
                                foreach (var t in fromData)
                                {
                                        WriteUInt32(t, toRaw);
                                }
                        }
                }
        }
}
