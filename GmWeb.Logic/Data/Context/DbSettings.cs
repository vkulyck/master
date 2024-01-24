using System.Configuration;

namespace GmWeb.Logic.Data.Context
{
    public class DbSettings
    {
        public static string ConnectionKey => "CURRENT_INSTANCE_DB";
        public static string ConnectionString => ConfigurationManager.ConnectionStrings[ConnectionKey].ConnectionString;
        public static string EncryptionKey => ConfigurationManager.AppSettings["Database.EncryptionKey"]?.ToString();
    }
}
