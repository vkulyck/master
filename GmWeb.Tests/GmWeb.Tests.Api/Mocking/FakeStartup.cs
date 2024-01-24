using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Startup = GmWeb.Web.Api.Startup;
using DataEntities = GmWeb.Tests.Api.Data.DataEntities;
using GmWeb.Tests.Api.Data;
using GmWeb.Tests.Api.Services;
using GmWeb.Logic.Utility.Extensions.Services;

namespace GmWeb.Tests.Api.Mocking
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        protected override void ConfigureAuth(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<DataEntities>();
            services.AddScoped<DataService>();
            services.AddScoped<CommonContextInitializer>();
            services.AddScoped<IdentityContextInitializer>();
        }
    }
}
