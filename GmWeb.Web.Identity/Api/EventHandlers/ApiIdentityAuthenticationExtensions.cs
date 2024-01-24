using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using GmWeb.Logic.Utility.Config;
using GmWeb.Web.Common.Auth.EventHandlers;

namespace GmWeb.Web.Identity.Api.EventHandlers;

public static class ApiIdentityAuthenticationExtensions
{
    public static IdentityCookiesBuilder AddHandledIdentityAuthentication(this IServiceCollection services, string defaultScheme, IConfiguration config)
    {
        var builder = services.AddAuthentication(defaultScheme);
        var cookieBuilder = new IdentityCookiesBuilder
        {
            ApplicationCookie = builder.AddHandledIdentityCookie<ApiApplicationAuthEventHandlers>(IdentityConstants.ApplicationScheme, config),
            ExternalCookie = builder.AddHandledIdentityCookie<ApiExternalAuthEventHandlers>(IdentityConstants.ExternalScheme, config),
            TwoFactorRememberMeCookie = builder.AddHandledIdentityCookie<ApiTfaRememberMeEventHandlers>(IdentityConstants.TwoFactorRememberMeScheme, config),
            TwoFactorUserIdCookie = builder.AddHandledIdentityCookie<ApiTfaUserIdEventHandlers>(IdentityConstants.TwoFactorUserIdScheme, config)
        };
        return cookieBuilder;
    }

    public static OptionsBuilder<CookieAuthenticationOptions> AddHandledIdentityCookie<TEventHandler>(
        this AuthenticationBuilder builder, string name, IConfiguration config
    )
        where TEventHandler : CookieAuthenticationEvents
    {
        var handlerAction = (CookieAuthenticationOptions opts) =>
        {
            opts.EventsType = typeof(TEventHandler);
        };
        builder.Services.AddScoped<TEventHandler>();
        return builder.AddIdentityCookie(name, config, handlerAction);
    }
}