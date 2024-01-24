using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Languages;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using GmWeb.Logic.Services.Datasets.Races;
using GmWeb.Logic.Services.Printing;
using GmWeb.Logic.Services.QRCode;
using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Identity;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Logic.Utility.Extensions.Services;
using StackExchange.Redis;

namespace GmWeb.Logic.Utility.Config;

public class GmStartup
{
    public IConfiguration Configuration { get; set; }
    public GmStartup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    protected virtual void ConfigureIdentityCore(IServiceCollection services)
        => this.ConfigureIdentityCore<GmIdentity>(services);
    protected virtual void ConfigureIdentityCore<TUser>(IServiceCollection services)
        where TUser : IdentityUser<Guid>
        => this.ConfigureIdentityCore<TUser>(services, default);
    protected virtual void ConfigureIdentityCore<TUser>(IServiceCollection services, Action<IdentityOptions> setupAction)
        where TUser : IdentityUser<Guid>
        => this.ConfigureIdentityBase(services, services.AddIdentityCore<TUser>(setupAction));

    protected virtual void ConfigureIdentity(IServiceCollection services)
        => this.ConfigureIdentity<GmIdentity,GmRole>(services);
    protected virtual void ConfigureIdentity<TUser,TRole>(IServiceCollection services)
        where TUser : IdentityUser<Guid> where TRole : IdentityRole<Guid>
        => this.ConfigureIdentity<TUser,TRole>(services, default);
    protected virtual void ConfigureIdentity<TUser,TRole>(IServiceCollection services, Action<IdentityOptions> setupAction)
        where TUser : IdentityUser<Guid> where TRole : IdentityRole<Guid>
        => this.ConfigureIdentityBase(services, services.AddIdentity<TUser, TRole>(setupAction));

    private void ConfigureIdentityBase(IServiceCollection services, IdentityBuilder builder)
    {
        services.Configure<IdentityOptions>(this.Configuration.GetSection("Identity"));

        builder
            .AddClaimsPrincipalFactory<GmClaimsFactory>()
            .AddEntityFrameworkStores<GmIdentityContext>()
            .AddUserStore<GmUserStore>()
            .AddUserManager<GmUserManager>()
            .AddSignInManager<GmSignInManager>()
            .AddDefaultTokenProviders()
        ;

        services
                .AddScoped<GmUserStore>()
                .AddScoped<GmPasswordHasher>()
                .AddScoped<GmUserManager>()
                .AddScoped<GmClaimsFactory>()
        ;

        services.ConfigureSqlDbContext<GmIdentityContext>(this.Configuration);
        services.ConfigureSqlDbContext<CarmaContext>(this.Configuration);
    }
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureGmWebOptions(this.Configuration);
        services.ConfigureGmCookieDefaults(this.Configuration);
        services.Configure<DatabaseConnectionOptions>(Configuration.GetSection("DatabaseConnections:Default"));
        services.Configure<PrinterOptions>(Configuration.GetSection("Services:Printing"));
        services.Configure<QRCodeOptions>(Configuration.GetSection("Services:QRCode"));
        services.Configure<LanguageMatchingOptions>(Configuration.GetSection("Services:Language"));
        services.Configure<LanguageOptions>(this.Configuration.GetSection("Services:Languages"));
        services.Configure<CountryOptions>(this.Configuration.GetSection("Services:Countries"));
        services.Configure<EthnicityMatchingOptions>(this.Configuration.GetSection("Services:Ethnicities"));
        services.Configure<EthnicityOptions>(this.Configuration.GetSection("Services:Ethnicities"));
        services.Configure<RaceOptions>(this.Configuration.GetSection("Services:Races"));
        services.Configure<DeltaOptions>(Configuration.GetSection("Services:Deltas"));
        services.Configure<CacheOptions>(this.Configuration.GetSection("RedisCache"));

        services.AddScoped<UserZplPrinterService>();
        services.AddScoped<IQRCodeGeneratorService, QRCodeGeneratorService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<EthnicityMatchingService>();
        services.AddSingleton<CountryService>();
        services.AddSingleton<EthnicityService>();
        services.AddSingleton<RaceService>();
        services.AddCountries();
        services.AddScoped<DeltaService>();
        services.AddTransient<RedisCache>();
        this.ConfigureDataProtection(services);
    }

    private void ConfigureDataProtection(IServiceCollection services)
    {
        var redisConfig = this.Configuration.GetSection("RedisCache").Get<CacheOptions>();
        var connString = RedisCache.GenerateConnectionString(redisConfig);
        var multiplexer = ConnectionMultiplexer.Connect(connString);
        services
            .AddDataProtection()
            .SetApplicationName(RedisCache.DataProtectionApplicationName)
            .PersistKeysToStackExchangeRedis(multiplexer)
        ;
    }
}
