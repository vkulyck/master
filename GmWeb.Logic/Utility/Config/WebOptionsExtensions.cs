using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GmWeb.Logic.Utility.Extensions.Hosting;
using GmWeb.Logic.Utility.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GmWeb.Logic.Utility.Config;

public static class WebOptionsExtensions
{
    private static readonly string GmWebOptionsKey = "GmWeb";
    private static readonly string GmWebConsoleColorKey = "Logging:Console:FormatterOptions";
    private static readonly string BaseCookieOptionsKey = "Auth:Cookies";
    private static readonly string DefaultCookieOptionsKey = $"{BaseCookieOptionsKey}:Default";

    #region Binding
    public static GmWebOptions GetWebOptions(this IConfiguration config)
        => config.GetSection(GmWebOptionsKey).Get<GmWebOptions>();
    public static CookieBuilder GetDefaultCookieBuilder(this IConfiguration config)
        => config.GetSection(DefaultCookieOptionsKey).Get<CookieBuilder>();
    public static CookieOptions GetDefaultCookieOptions(this IConfiguration config, HttpContext context)
        => config.GetDefaultCookieBuilder().Build(context);
    public static CookieOptions GetDefaultCookieOptions(this IConfiguration config, HttpContext context, DateTimeOffset expiresFrom)
        => config.GetDefaultCookieBuilder().Build(context, expiresFrom);
    public static void ApplyCookieDefaults<T>(this IConfiguration config, T instance)
        where T : CookieBuilder
        => config.Bind(DefaultCookieOptionsKey, instance);
    public static void ApplyCookieScheme<T>(this IConfiguration config, T instance, string name)
        where T : CookieBuilder
        => config.Bind($"{BaseCookieOptionsKey}:{name}", instance);
    #endregion

    #region Configuration

    public static IHostBuilder ConfigureGmSettings(this IHostBuilder builder)
    {
        return builder
            .ConfigureServices(services =>
            {

            })
        ;
    }
    public static IServiceCollection ConfigureGmWebOptions(this IServiceCollection services, IConfiguration config)

    {
        services.Configure<GmWebOptions>(config.GetSection(GmWebOptionsKey));
        services.Configure<GmConsoleFormatterOptions>(config.GetSection(GmWebConsoleColorKey));
        return services;
    }
    public static IServiceCollection ConfigureGmCookieDefaults(this IServiceCollection services, IConfiguration config)
    {
        var webopts = config.GetWebOptions();
        return services
            .Configure<CookieBuilder>(opts =>
            {
                config.ApplyCookieDefaults(opts);
                opts.Domain = webopts.CommonCookieDomain;
            })
            .Configure<CookieAuthenticationOptions>(opts =>
            {
                config.ApplyCookieDefaults(opts.Cookie);
                opts.LoginPath = webopts.Login.Path;
                opts.LogoutPath = webopts.Logout.Path;
                opts.ReturnUrlParameter = "ReturnUrl";
            })
        ;
    }
    public static IHostBuilder ConfigureGmConfigFiles(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration(cfg => cfg
            .AddDefaultConfigs()
            .AddExtendedConfigs()
            .AddSecretConfigs()
            .Save()
            .Build()
        );

    public static IHostBuilder ConfigureGmLogging(this IHostBuilder builder)
        => builder.ConfigureLogging(ConfigureLogging);

    public static void ConfigureLogging(this ILoggingBuilder builder) =>
        builder
            .ClearProviders()
            .AddConsole()
            .AddConsoleFormatter<GmConsoleFormatter, GmConsoleFormatterOptions>()
            .AddDebug()
        ;
    #endregion

}
