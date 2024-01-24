using System.Configuration;

namespace GmWeb.Logic.Utility.Phone
{
    public static class AWSCredentials
    {
        public static string AccessKey = ConfigurationManager.AppSettings["Sms.ApiClientId"];
        public static string SecretKey = ConfigurationManager.AppSettings["Sms.ApiSecretKey"];
    }
}
