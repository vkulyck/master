using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntakeData = GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake.IntakeData;

namespace GmWeb.Logic.Utility.Redis
{
    public partial class RedisCache
    {
        protected IRedisDatabase Db => this.Client.Db0;
        protected RedisCacheClient Client { get; private set; }
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CacheOptions _options;
        public const string DataProtectionApplicationName = "GmWeb";
        public static string GenerateConnectionString(CacheOptions settings)
        {
            var assignments = new List<string> { $"{settings.Host}:{settings.Port}" };
            if (!string.IsNullOrWhiteSpace(settings.Password))
                assignments.Add($"password={settings.Password}");
            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                assignments.Add($"user={settings.Username}");
            }
            if(settings.SSL)
            {
                assignments.Add("ssl=true");
                assignments.Add($"sslHost={settings.Host}");
            }
            string connString = string.Join(",", assignments);
            return connString;
        }

        public RedisCache(IOptions<CacheOptions> options)
        {
            this._options = options.Value;
            if (this._options == null)
                throw new ArgumentException($"No settings provided for RedisCache instance.");
            if (!this._options.Enabled)
                return;
            string connString = GenerateConnectionString(this._options);
            this.Client = new RedisCacheClient(
                new SinglePool(connString),
                new NewtonsoftSerializer(new JsonSerializerSettings()),
                new RedisConfiguration()
            );
        }

        protected string Key(IAccountToken token)
        {
            if (string.IsNullOrWhiteSpace(token.TokenID))
                return $"tokens:account:{token.AccountID}:{token.TokenType}";
            else
                return $"tokens:account:{token.AccountID}:{token.TokenType}:{token.TokenID}";
        }
        protected string DataKey(Guid accountID, string dataType)
        {
            return $"data:account:{accountID}:{dataType}";
        }
        protected string AccountTokenKeyPattern(Guid accountID) => $"tokens:account:{accountID}:*";
        public async Task<bool> RegisterAccountTokenAsync(IAccountToken token)
        {
            bool result = await this.Db.AddAsync(this.Key(token), string.Empty, expiresIn: token.Lifetime);
            return result;
        }
        public async Task<bool> RevokeAccountTokenAsync(IAccountToken token)
        {
            bool result = await this.Db.RemoveAsync(this.Key(token));
            return result;
        }
        public async Task<bool> RevokeAccountTokensAsync(Guid accountID)
        {
            string pattern = this.AccountTokenKeyPattern(accountID);
            var keys = await this.Db.SearchKeysAsync(pattern);
            long result = await this.Db.RemoveAllAsync(keys);
            return true; // Doesn't matter how many are removed
        }
        public async Task<bool> ValidateAccountTokenAsync(IAccountToken token)
        {
            bool exists = await this.Db.ExistsAsync(this.Key(token));
            return exists;
        }

        public async Task<bool> StoreIntakeDataAsync(Guid AccountID, IntakeData Data)
        {
            var key = this.DataKey(AccountID, "intake");
            var success = await this.Db.AddAsync(key, Data, TimeSpan.FromHours(24));
            return success;
        }
        public async Task<bool> ClearIntakeDataAsync(Guid AccountID)
        {
            var key = this.DataKey(AccountID, "intake");
            var success = await this.Db.RemoveAsync(key);
            return success;
        }

        public async Task<IntakeData> LoadIntakeDataAsync(Guid AccountID)
        {
            var key = this.DataKey(AccountID, "intake");
            var exists = await this.Db.ExistsAsync(key);
            if(exists)
                return await this.Db.GetAsync<IntakeData>(key);
            return default;
        }
    }
}
