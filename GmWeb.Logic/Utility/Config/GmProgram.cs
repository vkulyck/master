using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GmWeb.Logic.Utility.Extensions.Hosting;
using GmWeb.Logic.Utility.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GmWeb.Logic.Utility.Config;

public class GmProgram
{
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        return builder
            .ConfigureGmConfigFiles()
            .ConfigureGmLogging()
        ;
    }

    public static IHostBuilder CreateAppHostBuilder<TStartup>()
        where TStartup : class
        => CreateAppHostBuilder<TStartup>(new string[] { });
    public static IHostBuilder CreateAppHostBuilder<TStartup>(string[] args)
        where TStartup : class
        => CreateHostBuilder(args).UseStartup<TStartup>();

    public static IHostBuilder CreateWebHostBuilder<TStartup>()
        where TStartup : class
        => CreateWebHostBuilder<TStartup>(new string[] { });
    public static IHostBuilder CreateWebHostBuilder<TStartup>(string[] args)
        where TStartup : class
        => CreateHostBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureKestrel(opts => { })
                    .UseWebRoot("wwwroot")
                    .UseStartup<TStartup>()
                ;
            });
}
