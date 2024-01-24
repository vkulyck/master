using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace GmWeb.Web.Api.Utility.Config
{
    public static class SwaggerHelper
    {
        public static string AppVersion => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public static string AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version.ToString(4);
        public static void ConfigureSwagger(this IServiceCollection services)
            => ConfigureService(services);
        public static string BuildEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static void ConfigureService(IServiceCollection services)
        {
            string sch = JwtBearerDefaults.AuthenticationScheme;
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GM Carma API",
                    Version = "v1",
                    Description = $"Goodmojo CARMA API | API Version {AppVersion} | Assembly Version {AssemblyVersion} | {BuildEnvironment} Environment"
                });

                s.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = $"JWT {HeaderNames.Authorization} header using the {sch} scheme (Example: '{sch} 12345abcdef')",
                    Name = HeaderNames.Authorization,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });
            services.AddSwaggerGenNewtonsoftSupport();
        }
    }
}
