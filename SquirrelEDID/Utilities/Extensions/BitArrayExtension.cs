using System;
using System.Collections;

namespace SquirrelEDID.Utilities.Extensions
{
    public static class BitArrayExtension
    {
        public static byte ToByte(this BitArray arr, bool adjustSize = false)
        {
            if (!adjustSize && arr.Count != 8)
                throw new ArgumentException("Invalid number of bits!");

            int l = arr.Length / 8;
            if (arr.Length % 8 > 0)
                l += 1;

            byte[] b = new byte[l];
            arr.CopyTo(b, 0);

            return b[0];
        }

        public static string GetBinary(this BitArray arr)
        {
            string str = "";
            for (int i = arr.Length - 1; i >= 0; i--)
                str += arr[i] ? "1 " : "0 ";
            return str;
        }
    }
}
