using GmWeb.Web.Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GmWeb.Web.Scheduler.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKendoDemo(this IServiceCollection services)
        {
            foreach (var service in DemoServices.GetServices())
            {
                services.Add(service);
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
