using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GmWeb.Common.Crypto
{
    public static class SymmetricEncryptor
    {
        private static readonly int KeyBits = 0x100;
        private static readonly int KeyBytes = KeyBits / 8;
        private static readonly int IVBits = 0x100 / 2;
        private static readonly int IVBytes = IVBits / 8;

        public static string GenerateKey() => GenerateKey64();
        public static string GenerateKey64() => GenerateKey64(KeyBytes);
        public static string GenerateKey64(int numBytes)
        {
            var random = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[numBytes];
            random.GetBytes(bytes);
            string key = Base64.FromBytes(bytes);
            return key;
        }
        public static string GenerateKey32() => GenerateKey32(KeyBytes);
        public static string GenerateKey32(int numBytes)
        {
            var random = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[numBytes];
            random.GetBytes(bytes);
            string key = Base32.FromBytes(bytes);
            return key;
        }

        public static string GenerateKey(string seed) => GenerateKey64(seed);
        public static string GenerateKey64(string seed)
        {
            byte[] seedBytes = Encoding.UTF8.GetBytes(seed);
            int offset = 4 - seedBytes.Length % 4;
            if (offset != 0)
            {
                byte[] padding = new byte[offset];
                byte c = Encoding.UTF8.GetBytes(" ")[0];
                for (int i = 0; i < offset; i++)
                    padding[i] = c;
                seedBytes = seedBytes.Concat(padding).ToArray();
            }
            int seedNumber = 0;
            for (int i = 0; i < seedBytes.Length / 4; i++)
            {
                int n = BitConverter.ToInt32(seedBytes, i * 4);
                seedNumber ^= n;
            }
            var random = new Random(seedNumber);
            byte[] keyBytes = new byte[KeyBytes];
            random.NextBytes(keyBytes);
            string key = Base64.FromBytes(keyBytes);
            return key;
        }

        private static string GenerateIV() => GenerateKey64(IVBytes);

        public static string EncryptObject(object cipher, string key)
        {
            if (cipher == null)
                return null;
            return Encrypt(cipher.ToString(), key);
        }

        public static string Encrypt(string data, string key)
        {
            if (data == null)
                return null;
            if (key == null)
                throw new ArgumentNullException("key");
            string iv = GenerateIV();
            byte[]
                keyBytes = Base64.ToBytes(key),
                ivBytes = Base64.ToBytes(iv)
            ;
            string cipher = null;
            using (var algorithm = new AesManaged { Key = keyBytes, IV = ivBytes, Mode = CipherMode.CBC })
            {
                var encryptor = algorithm.CreateEncryptor(keyBytes, ivBytes);
                var memoryStream = new MemoryStream();
                memoryStream.Write(ivBytes, 0, ivBytes.Length);
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(data);
                }

                cipher = Base64.FromBytes(memoryStream.ToArray());
            }
            return cipher;
        }

        public static string DecryptObject(object cipher, string key)
        {
            if (cipher == null)
                return null;
            return Decrypt(cipher.ToString(), key);
        }

        public static string Decrypt(string cipher, string key)
        {
            if (cipher == null)
                return null;
            if (key == null)
                throw new ArgumentNullException("key");
            byte[] keyBytes = Base64.ToBytes(key);
            string data = null;
            using (var memoryStream = new MemoryStream(Base64.ToBytes(cipher)))
            {
                byte[] ivBytes = new byte[IVBytes];
                memoryStream.Read(ivBytes, 0, IVBytes);
                using (var algorithm = new AesManaged { Key = keyBytes, IV = ivBytes, Mode = CipherMode.CBC })
                {
                    var decryptor = algorithm.CreateDecryptor(keyBytes, ivBytes);
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream))
                    {
                        data = reader.ReadToEnd();
                    }
                }
            }
            return data;
        }
    }
}
