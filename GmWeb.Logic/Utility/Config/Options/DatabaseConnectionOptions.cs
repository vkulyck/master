using Microsoft.Extensions.Configuration;

namespace GmWeb.Logic.Utility.Config
{
    public static class DatabaseConnectionSettingsExtensions
    {
        public static DatabaseConnectionOptions Db(this IConfiguration config)
            => config.Db("Default");
        public static DatabaseConnectionOptions Db(this IConfiguration config, string connectionName)
            => config
                .GetSection("DatabaseConnections")
                .GetSection(connectionName)
                .Get<DatabaseConnectionOptions>()
        ;
    }
    public class DatabaseConnectionOptions
    {
        public int? CommandTimeout { get; set; } = 60;
        public string EncryptionKey { get; set; }
    }
}
