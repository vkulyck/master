using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Context.Identity;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Web;
using GmWeb.Web.Common.Auth.Attributes;
using GmWeb.Web.Common.Auth.EventHandlers;
using GmWeb.Web.Common.Auth.Services.Passport;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using GmWeb.Web.Common.Utility;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GmWeb.Web.RHI;
public class Startup : GmStartup
{
    public Startup(IConfiguration configuration)
        : base(configuration)
    { }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<IntakeOptions>(this.Configuration.GetSection("Intake"));
        services.Configure<JwtAuthOptions>(this.Configuration.GetSection("Auth:Jwt"));
        services.Configure<BypassAuthOptions>(BypassAuthExtensions.AuthenticationScheme, this.Configuration.GetSection("Auth:Bypass"));
        this.ConfigureIdentityCore<GmIdentity>(services, opts =>
            {
                opts.ClaimsIdentity = new ClaimsIdentityOptions
                {
                    EmailClaimType = ClaimTypes.Email,
                    SecurityStampClaimType = ClaimTypes.Sid,
                    UserIdClaimType = ClaimTypes.NameIdentifier
                };
            })
        ;

        services.AddBypassAuthentication();
        services.AddIdentityAuthentication(IdentityConstants.ApplicationScheme, this.Configuration);
        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<GmIdentity>>();
        services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => {
            var provider = (ServerAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>();
            return provider;
        });

        var mvcBuilder = services.AddControllersWithViews();
        services.AddServerSideBlazor();
        services.AddRazorPages()
            .AddMvcOptions(options => { })
        ;
        services.AddKendo();
        services.AddTelerikBlazor();
        services
            .AddMvc(opts =>
            {
                opts.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddSessionStateTempDataProvider()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonStringBooleanConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringNullableConverterFactory());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
        ;
        services
            .AddDistributedMemoryCache()
            .AddSession(opts =>
            {
                opts.Cookie.IsEssential = true;
            })
        ;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseForwardedHeaders();

        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new CompositeFileProvider(
                env.WebRootFileProvider,
                new ManifestEmbeddedFileProvider(typeof(Startup).Assembly, "wwwroot")
            )
        });
        app.UseStaticFiles();
        app.UseWebSockets();

        app.UseSession();
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}/{id?}",
                defaults: new { action = "Index" }
            );
            if (this.Configuration.GetValue<bool>("EnableRazorFallback"))
                endpoints.MapFallbackToPage("/Root/_Host");
            else
                endpoints.MapFallbackToController(action: "Index", controller: "Intake");
        });
        app.UseHostFiltering();
        MappingFactory.Instance
            .AddProfile<RhiMappingProfile>()
        ;
    }
}
