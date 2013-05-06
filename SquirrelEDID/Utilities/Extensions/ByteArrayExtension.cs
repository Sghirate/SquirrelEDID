using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelEDID.Utilities.Extensions
{
    public static class ByteArrayExtension
    {
        private static bool CheckMatch(byte[] data, byte[] pattern, int start)
        {
            int p = 0;
            while (start < data.Length && p < pattern.Length)
                if (data[start++] != pattern[p++])
                    return false;

            return true;
        }

        public static bool HasPatternAt(this byte[] data, byte[] pattern, int position)
        {
            return CheckMatch(data, pattern, position);
        }

        public static bool StartsWithPattern(this byte[] data, byte[] pattern)
        {
            return data.HasPatternAt(pattern, 0);
        }

        public static bool EndsWithPattern(this byte[] data, byte[] pattern)
        {
            return data.HasPatternAt(pattern, data.Length - pattern.Length);
        }

        public static bool HasPattern(this byte[] data, byte[] pattern)
        {
            for (int i = 0; i < data.Length; i++)
                if (CheckMatch(data, pattern, i))
                    return true;
            return false;
        }

        public static Tuple<int, int> FindPattern(this byte[] data, byte[] pattern)
        {
            for (int i = 0; i < data.Length; i++)
                if (CheckMatch(data, pattern, i))
                    return new Tuple<int, int>(i, i + pattern.Length - 1);
            return null;
        }

        public static List<Tuple<int, int>> FindPatterns(this byte[] data, byte[] pattern)
        {
            List<Tuple<int, int>> toRet = new List<Tuple<int, int>>();
            for (int i = 0; i < data.Length; i++)
                if (CheckMatch(data, pattern, i))
                    toRet.Add(new Tuple<int, int>(i, i + pattern.Length - 1));
            return toRet;
        }

        public static string GetHexString(this byte[] data, int start = 0, int length = -1)
        {
            if (length < 0)
                length = data.Length;

            string str = "";
            for (int i = start; i < start + length; i++)
                str += data[i].ToString("X2") + " ";

            return str.Trim();
        }

        public static byte[] GetBytesFromHex(this string str, int length = -1)
        {
            char[] valid = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

            List<byte> buffer = new List<byte>();
            byte cur = 0;
            bool first = true;
            for (int i = 0; i < str.Length; i++)
            {
                char c = Char.ToLower(str[i]);
                if (c == 'x')
                {
                    first = true;
                    cur = 0;
                }
                else
                {
                    for (int j = 0; j < valid.Length; j++)
                        if (valid[j] == c)
                        {
                            if (first)
                            {
                                cur = (byte)j;
                                first = false;
                            }
                            else
                            {
                                cur *= 16;
                                cur += (byte)j;
                                first = true;

                                buffer.Add(cur);
                            }
                        }
                }
                if (length > 0 && buffer.Count >= length)
                    break;
            }
            return buffer.ToArray();
        }

        public static string GetString(this byte[] data, int start = 0, int length = -1)
        {
            if (length < 0)
                length = data.Length;

            byte[] sub = new byte[length];
            for (int i = start; i < start + length; i++)
                sub[i - start] = data[i];

            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(sub).Trim();
        }

        public static byte[] GetBytes(this string str)
        {
            if (str == null)
                return new byte[0];

            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(str);
        }

        public static byte[] GetBytesUTF8(this string str)
        {
            if (str == null)
                return new byte[0];

            UTF8Encoding enc = new UTF8Encoding();
            return enc.GetBytes(str);
        }

        public static string GetNullTermString(this byte[] data, int start = 0, int maxLength = -1)
        {
            List<byte> bytes = new List<byte>();

            if (maxLength < 0)
                maxLength = data.Length;

            int i = start;
            while (i < data.Length && data[i] != 0 && i - start < maxLength)
                bytes.Add(data[i++]);

            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(bytes.ToArray()).Trim();
        }

        public static byte GetParity(this byte[] data, int start = 0, int length = -1)
        {
            BitArray toRet = new BitArray(8);

            if (length < 0)
                length = data.Length;
            byte[] sub = new byte[length];
            for (int i = start; i < start + length; i++)
                sub[i - start] = data[i];

            BitArray source = new BitArray(sub);
            for (int i = 0; i < source.Length; i++)
                if (source[i])
                    toRet[i % 8] = !toRet[i % 8];

            return toRet.ToByte();
        }

        public static string GetBinary(this byte[] data)
        {
            return new BitArray(data).GetBinary();
        }

        public static string GetBinary(this byte data)
        {
            return new BitArray(new byte[] { data }).GetBinary();
        }
    }
}
