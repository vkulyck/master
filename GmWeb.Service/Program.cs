using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Extensions.Enums;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using GmWeb.Logic.Services.Datasets.Races;
using GmWeb.Service.Logging;
using GmWeb.Logic.Utility.Extensions.Hosting;

namespace GmWeb.Service;
public class Program : GmProgram
{
    static async Task Main(string[] args)
    {
        using IHost host = CreateAppHostBuilder<Startup>(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var svc = scope.ServiceProvider.GetService<UserZplPrinterService>();
            await svc.RunAsync();
        }

        await host.RunAsync();
    }
}
