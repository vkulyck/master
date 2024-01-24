using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Email;
using GmWeb.Web.Common.Auth.Services.Passport;
using GmWeb.Web.Identity.Api.EventHandlers;
using GmWeb.Web.Identity.Data;
using GmWeb.Web.Identity.Utility;
using GmWeb.Web.Identity.Utility.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;
using GmWeb.Web.Common.Auth.EventHandlers;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Identity.Api;
using GmWeb.Web.Identity.Api.WebService;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;

namespace GmWeb.Web.Identity;
public class Startup : GmStartup
{
    public Startup(IConfiguration configuration)
        : base(configuration)
    { }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<EmailSettings>(this.Configuration.GetSection("Email"));
        services.Configure<DebugSettings>(this.Configuration.GetSection("Debug"));
        services.Configure<IdentityOptions>(this.Configuration.GetSection("Identity"));
        services.Configure<JwtAuthOptions>(this.Configuration.GetSection("Auth:Jwt"));
        services.AddScoped<ApiService>();
        services.AddScoped<ApiUserStore>();
        services.AddScoped<GmWeb.Logic.Utility.Identity.GmPasswordHasher>();
        services.AddScoped<ApiUserManager>();
        services.AddScoped<ApiSignInManager>();
        services.AddScoped<ApiClaimsFactory>();
        var builder = services
            .AddIdentityCore<GmIdentity>(opts =>
            {
                opts.ClaimsIdentity = new ClaimsIdentityOptions
                {
                    EmailClaimType = ClaimTypes.Email,
                    SecurityStampClaimType = ClaimTypes.Sid,
                    UserIdClaimType = ClaimTypes.NameIdentifier
                };
            })
            .AddClaimsPrincipalFactory<ApiClaimsFactory>()
            .AddUserStore<ApiUserStore>()
            .AddUserManager<ApiUserManager>()
            .AddSignInManager<ApiSignInManager>()
            .AddDefaultTokenProviders()
        ;

        services.AddDbContext<CarmaContext>();
        services.AddSingleton<IEmailSender, EmailSender>();

        services.AddScoped<IPassportService, ApiService>();
        services.AddHandledIdentityAuthentication(IdentityConstants.ApplicationScheme, this.Configuration);

        services.AddAuthorization(options =>
        {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
        });

        services
            .AddControllersWithViews()
        ;

        services.AddRazorPages()
            .AddMvcOptions(options => { })
            .AddMicrosoftIdentityUI()
        ;

        services
            .AddMvc()
            .AddSessionStateTempDataProvider()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
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

        app.UseSession();

        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Scheduler}/{action=Index}/{id?}"
            );
        });
        app.UseHostFiltering();
        MappingFactory.Instance
            .AddProfile<IdentityMappingProfile>()
        ;
    }
}
