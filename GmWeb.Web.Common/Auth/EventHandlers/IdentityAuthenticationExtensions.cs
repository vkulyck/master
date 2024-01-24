using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Redis;

namespace GmWeb.Web.Common.Auth.EventHandlers;
public static class IdentityAuthenticationExtensions
{
    public static IdentityCookiesBuilder AddIdentityAuthentication(this IServiceCollection services, string defaultScheme, IConfiguration config)
    {
        var builder = services.AddAuthentication(defaultScheme);
        var cookieBuilder = new IdentityCookiesBuilder
        {
            ApplicationCookie = builder.AddIdentityCookie(IdentityConstants.ApplicationScheme, config),
            ExternalCookie = builder.AddIdentityCookie(IdentityConstants.ExternalScheme, config),
            TwoFactorRememberMeCookie = builder.AddIdentityCookie(IdentityConstants.TwoFactorRememberMeScheme, config),
            TwoFactorUserIdCookie = builder.AddIdentityCookie(IdentityConstants.TwoFactorUserIdScheme, config)
        };
        return cookieBuilder;
    }

    public static OptionsBuilder<CookieAuthenticationOptions> AddIdentityCookie(
        this AuthenticationBuilder builder, string name, IConfiguration config
    )
        => builder.AddIdentityCookie(name, config, default);
    public static OptionsBuilder<CookieAuthenticationOptions> AddIdentityCookie(this AuthenticationBuilder builder, 
        string name, IConfiguration config, Action<CookieAuthenticationOptions> configureCookie
    )
    {
        var webopts = config.GetWebOptions();
        builder.Services.Configure<CookieAuthenticationOptions>(name, opts =>
        {
            config.ApplyCookieDefaults(opts.Cookie);
            opts.Cookie.Domain = webopts.CommonCookieDomain;
        });
        builder.Services.Configure<CookieAuthenticationOptions>(name, config.GetSection(name));
        builder.AddCookie(name, o =>
        {
            config.ApplyCookieScheme(o.Cookie, name);
            o.Cookie.Name = name;
            o.Cookie.MaxAge = o.ExpireTimeSpan;
            if (configureCookie is not null)
                configureCookie(o);
        });
        
        return new OptionsBuilder<CookieAuthenticationOptions>(builder.Services, name);
    }
}
