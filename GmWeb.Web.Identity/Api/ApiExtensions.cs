using System;

namespace GmWeb.Web.Identity.Api;

public static class ApiExtensions
{
    public static TfaProviderType ToProvider(this string providerName)
        => Enum.TryParse(providerName, out TfaProviderType result) 
        ? result 
        : throw new ArgumentException(nameof(providerName))
    ;
}
