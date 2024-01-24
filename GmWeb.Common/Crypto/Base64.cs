using System;
using System.Text.RegularExpressions;

namespace GmWeb.Common.Crypto
{
    public class Base64
    {
        public static string FromBytes(byte[] bytes) => Convert.ToBase64String(bytes);
        public static byte[] ToBytes(string encoded) => Convert.FromBase64String(encoded);

        public static bool IsBase64String(string base64)
        {
            if (base64.Length % 4 != 0)
                return false;
            if (!Regex.IsMatch(base64, @"^[A-Za-z0-9\+\/]+={0,2}$"))
                return false;
            return true;
        }

        public static int EncodingLength(int nBytes)
        {
            int fullDigits = 4 * nBytes / 3;
            if (nBytes % 3 == 0)
                return fullDigits;
            // Result is padded to nearest multiple of 4
            int len = fullDigits + 4 - (fullDigits % 4);
            return len;
        }
    }
}
