using GmWeb.Logic.Utility.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;
using DbOptionsConnector = System.Func<string, System.Action<Microsoft.EntityFrameworkCore.DbContextOptionsBuilder>>;
using SqlServerOptionsAction = System.Action<Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder>;

namespace GmWeb.Logic.Utility.Extensions.Services;
public static class ServiceExtensions
{
    public static IServiceCollection ConfigureSqlDbContext<TContext>(
        this IServiceCollection services, IConfiguration config, SqlServerOptionsAction sqlServerOptionsAction
    )
        where TContext : DbContext
    {
        var connSettings = config.GetSection("DatabaseConnections:Default").Get<DatabaseConnectionOptions>();
        SqlServerOptionsAction wrapped = (SqlServerDbContextOptionsBuilder sqlServerOptions)
            => sqlServerOptionsAction(sqlServerOptions.CommandTimeout(connSettings?.CommandTimeout))
        ;
        DbOptionsConnector connector = (string connString)
            => (DbContextOptionsBuilder options)
                => options.UseSqlServer(connString, wrapped)
        ;
        return services.ConfigureDbContext<TContext>(config, connector);
    }

    public static IServiceCollection ConfigureSqlDbContext<TContext>(this IServiceCollection services, IConfiguration config)
        where TContext : DbContext
    {
        var connSettings = config.GetSection("DatabaseConnections:Default").Get<DatabaseConnectionOptions>();
        SqlServerOptionsAction sqlServerOptionsAction = (SqlServerDbContextOptionsBuilder sqlServerOptions)
            => sqlServerOptions.CommandTimeout(connSettings?.CommandTimeout)
        ;
        DbOptionsConnector connector = (string connString)
            => (DbContextOptionsBuilder options)
                => options.UseSqlServer(connString, sqlServerOptionsAction)
        ;
        return services.ConfigureDbContext<TContext>(config, connector);
    }


    public static IServiceCollection ConfigureSqliteDbContext<TContext>(this IServiceCollection services, IConfiguration config)
        where TContext : DbContext
    => services.ConfigureDbContext<TContext>(config, (string connString)
        => (DbContextOptionsBuilder options)
            => options.UseSqlite(connString)
    );

    public static IServiceCollection ConfigureDbContext<TContext>(this IServiceCollection services, IConfiguration config, DbOptionsConnector connector)
        where TContext : DbContext
    {
        var regex = new Regex($@"^(?:Gm)?(?<name>[A-Z]\w+)Context$", RegexOptions.Compiled);
        string ctxName = regex.Replace(typeof(TContext).Name, "${name}");
        string connString = config.GetConnectionString(ctxName);
        var optionsAction = connector(connString);
        return services.AddDbContext<TContext>(optionsAction);
    }

    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<TOptions> configureOptions) where TOptions : class
    {
        return services
            .Configure<TOptions>(name, config)
            .Configure<TOptions>(name, configureOptions)
        ;
    }
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions> configureOptions, IConfiguration config) where TOptions : class
    {
        return services
            .Configure<TOptions>(name, config)
            .Configure<TOptions>(name, configureOptions)
        ;
    }
}