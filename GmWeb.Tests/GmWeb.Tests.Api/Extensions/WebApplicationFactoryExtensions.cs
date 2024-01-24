using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using GmWeb.Web.Common.Auth.Services.JwtAuth;
using GmWeb.Tests.Api.Data;
using GmWeb.Logic.Utility.Config;

namespace GmWeb.Tests.Api.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateDefaultClient<T>(this WebApplicationFactory<T> factory, string baseAddress, params DelegatingHandler[] handlers)
            where T : class
        => factory.CreateDefaultClient(new Uri(baseAddress), handlers);
        public static (HttpClient Client, CookieContainer CookieContainer) CreateApiClient<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var webOptions = factory.Services.GetService<IOptions<GmWebOptions>>().Value;
            var container = new CookieContainer();
            var handler = new CookieContainerHandler(container);
            var client = factory.CreateDefaultClient(webOptions.Api.BaseUri, handler);
            return (client, container);
        }
        private static WebApplicationFactory<T> WithBypassAuth<T>(this WebApplicationFactory<T> factory, DataEntities entities) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddJwtAuthentication();
                    services.AddBypassAuthentication(u =>
                    {
                        u.Enabled = true;
                        u.AuthenticationEmail = entities.AdminEmail;
                    });
                });
            });
        }        
        
        public static WebApplicationFactory<T> WithJwtAuth<T>(this WebApplicationFactory<T> factory) where T : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddJwtAuthentication();
                    services.AddBypassAuthentication(u =>
                    {
                        u.Enabled = false;
                    });
                });
            });
        }
        public static HttpClient CreateJwtClient<T>(this WebApplicationFactory<T> factory) where T : class
            => factory.WithJwtAuth().CreateApiClient().Client;
               
        public static (HttpClient, CookieContainer) CreateJwtCookieClient<T>(this WebApplicationFactory<T> factory) where T : class
            => factory.WithJwtAuth().CreateApiClient();

        public static HttpClient CreateAdminClient<T>(this WebApplicationFactory<T> factory, DataEntities entities) where T : class
            => factory.WithBypassAuth(entities).CreateApiClient().Client;
    }
}
