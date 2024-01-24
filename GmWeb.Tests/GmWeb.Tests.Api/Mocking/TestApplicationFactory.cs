using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using GmWeb.Logic.Utility.Config;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using GmWeb.Web.Common.Auth.Services.JwtAuth;
using GmWeb.Tests.Api.Data;
using ApiProgram = GmWeb.Web.Api.Program;

namespace GmWeb.Tests.Api.Mocking
{
    public class TestApplicationFactory : TestApplicationFactory<FakeStartup> { }
    public class TestApplicationFactory<TTestStartup> : WebApplicationFactory<TTestStartup> where TTestStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
            => this.CreateHostBuilder(new string[] { });
        protected IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = ApiProgram.CreateWebHostBuilder<TTestStartup>(args);
            return builder;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            return base.CreateHost(builder);
        }
    }
}
