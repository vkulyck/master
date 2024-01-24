using System.Security.Cryptography;

namespace GmWeb.Common.Crypto
{
    /// <summary>
    /// Source code from: https://cmatskas.com/-net-password-hashing-using-pbkdf2/
    /// </summary>
    public class Hasher
    {
        private const int SaltByteSize = 24;
        private const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        private const int Pbkdf2Iterations = 1000;
        public static int HashLength => Base64.EncodingLength(SaltByteSize) + Base64.EncodingLength(HashByteSize);

        public static string HashPassword(string password) => ComputeDataHash(password);
        public static string ComputeDataHash(string data)
        {
            if (data == null)
                return null;
            var cryptoProvider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);
            string salt64 = Base64.FromBytes(salt);

            byte[] hash = GetPbkdf2Bytes(data, salt, Pbkdf2Iterations, HashByteSize);
            string hash64 = Base64.FromBytes(hash);

            string salthash = salt64 + hash64;
            return salthash;
        }

        public static bool ValidatePassword(string password, string salthash) => ValidateHashData(password, salthash);
        public static bool ValidateHashData(string data, string salthash)
        {
            int saltLength = Base64.EncodingLength(SaltByteSize);
            string salt = salthash.Substring(0, saltLength);
            byte[] saltBytes = Base64.ToBytes(salt);

            string hash = salthash.Substring(saltLength);
            byte[] hashBytes = Base64.ToBytes(hash);

            byte[] testHash = GetPbkdf2Bytes(data, saltBytes, Pbkdf2Iterations, hashBytes.Length);
            bool isValid = SlowEquals(hashBytes, testHash);
            return isValid;
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt)
            {
                IterationCount = iterations
            };
            byte[] hashBytes = pbkdf2.GetBytes(outputBytes);
            return hashBytes;
        }
    }
}
