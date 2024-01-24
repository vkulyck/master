using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GmWeb.Web.Common.Auth.Services.BypassAuth
{
    public static class BypassAuthExtensions
    {
        public const string AuthenticationScheme = "Bypass";
        public static AuthenticationBuilder AddBypassAuthentication(
            this IServiceCollection services,
            Action<BypassAuthOptions> configureOptions = default(Action<BypassAuthOptions>)
        )
            => services
                .AddAuthentication(BypassAuthExtensions.AuthenticationScheme)
                .AddScheme<BypassAuthOptions, BypassAuthHandler>(BypassAuthExtensions.AuthenticationScheme, "Authentication Bypass", opts =>
                {
                    opts.IgnoreAuthenticationIfAllowAnonymous = true;
                    opts.IgnoreAuthenticationIfAlreadyAuthenticated = true;
                    if (configureOptions != default(Action<BypassAuthOptions>))
                        configureOptions(opts);
                })
        ;
    }
}
