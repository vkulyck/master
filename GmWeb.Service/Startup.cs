using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Extensions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GmWeb.Service
{
    public class Startup : GmStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        { }

        protected virtual void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<ClientImportOptions>(Configuration.GetSection("Services:Importing:Clients"));
            services.Configure<ActivityImportOptions>(Configuration.GetSection("Services:Importing:Activities"));
        }
        protected virtual void ConfigureData(IServiceCollection services)
        {
            services.ConfigureSqlDbContext<CarmaContext>(this.Configuration);
        }
        protected virtual void ConfigureServiceProviders(IServiceCollection services)
        {
            services.AddScoped<ClientImportService>();
            services.AddScoped<ActivityImportService>();
            services.AddScoped<ClientSanitizerService>();
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            ConfigureSettings(services);
            ConfigureData(services);
            ConfigureServiceProviders(services);
        }
    }
}
