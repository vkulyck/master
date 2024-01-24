using Microsoft.SqlServer.Server;
using Base64 = GmWeb.Common.Crypto.Base64;
using Hasher = GmWeb.Common.Crypto.Hasher;
using SymmetricEncryptor = GmWeb.Common.Crypto.SymmetricEncryptor;

namespace GmWeb.Db.Common
{
    public static class CryptoExports
    {
        [SqlFunction(
                Name = "GenerateRandomKey",
                IsPrecise = true,
                IsDeterministic = true,
                DataAccess = DataAccessKind.None,
                SystemDataAccess = SystemDataAccessKind.None
            )]
        public static string GenerateRandomKey() => SymmetricEncryptor.GenerateKey();

        [SqlFunction(
            Name = "GenerateKey",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static string GenerateKey(string Seed) => SymmetricEncryptor.GenerateKey(Seed);

        [SqlFunction(
            Name = "AesEncrypt",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static string AesEncrypt(string data, string key) => SymmetricEncryptor.Encrypt(data, key);

        [SqlFunction(
            Name = "AesDecrypt",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static string AesDecrypt(string data, string key) => SymmetricEncryptor.Decrypt(data, key);

        [SqlFunction(
            Name = "BytesToBase64",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static string BytesToBase64(byte[] bytes) => Base64.FromBytes(bytes);


        [SqlFunction(
            Name = "Base64ToBytes",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static byte[] Base64ToBytes(string encoded) => Base64.ToBytes(encoded);


        [SqlFunction(
            Name = "ComputeDataHash",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static string ComputeDataHash(string data) => Hasher.ComputeDataHash(data);

        [SqlFunction(
            Name = "ValidateData",
            IsPrecise = true,
            IsDeterministic = true,
            DataAccess = DataAccessKind.None,
            SystemDataAccess = SystemDataAccessKind.None
        )]
        public static bool ValidateHashData(string data, string salthash) => Hasher.ValidateHashData(data, salthash);
    }
}
