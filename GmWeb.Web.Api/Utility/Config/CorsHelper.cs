using Microsoft.Extensions.DependencyInjection;

namespace GmWeb.Web.Api.Utility.Config
{
    public static class CorsHelper
    {
        public static void ConfigureCors(this IServiceCollection services)
            => ConfigureService(services);
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                ;
            }));
        }
    }
}
