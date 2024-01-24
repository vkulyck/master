using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GmWeb.Common
{
    public static class AppIdentityConfig
    {
        public static AppIdentityType ConfiguredIdentityType
        {
            get
            {
                string configValue = ConfigurationManager.AppSettings["Database.IdentityType"]?.ToString();
                if (string.IsNullOrWhiteSpace(configValue))
                    return AppIdentityType.User;
                var parsed = (AppIdentityType)Enum.Parse(typeof(AppIdentityType), configValue);
                return parsed;
            }
        }
        public static IEnumerable<AppIdentityType> IdentityTypes => Enum.GetValues(typeof(AppIdentityType)).Cast<AppIdentityType>();
    }
}
