using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Services.Importing.Clients;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Email;
using GmWeb.Logic.Utility.Extensions.Services;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using GmWeb.Web.Common.Auth.Services.JwtAuth;
using GmWeb.Web.Api.Utility;
using GmWeb.Web.Api.Utility.Attributes;
using GmWeb.Web.Api.Utility.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SwaggerOptions = GmWeb.Web.Api.Utility.SwaggerOptions;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using StringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace GmWeb.Web.Api;

public class Startup : GmStartup
{
    public IWebHostEnvironment Env { get; }
    public bool HttpLoggingEnabled => this.Configuration.GetValue<bool>("HttpLogging:Enabled", false);
    public string ProfilePictureRoot =>
        this.Configuration.GetSection("ProfilePictureRoot").Exists()
        ? this.Configuration.GetSection("ProfilePictureRoot").Value
        : Path.Combine(this.Env.WebRootPath, "Images", "UserProfile")
    ;
    public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration)
    {
        this.Env = env;
    }

    protected virtual void ConfigureMvc(IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddMvcCore(options =>
        {
            options.AllowEmptyInputInBodyModelBinding = true;
            options.Conventions.Add(new KebabCaseActionConvention());
            options.Conventions.Add(new KebabCaseControllerConvention());
            options.Conventions.Add(new ControllerNameAttribute.Convention());
            options.Conventions.Add(new ActionNameConvention());
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
        services.ConfigureCors();
        services.ConfigureSwagger();
    }
    protected virtual void ConfigureSettings(IServiceCollection services)
    {
        services.Configure<CookieAuthenticationOptions>(
            IdentityConstants.TwoFactorRememberMeScheme, 
            this.Configuration.GetSection("Auth:Cookies:Identity.TwoFactorRememberMe")
        );
        services.Configure<IdentityOptions>(this.Configuration.GetSection("Identity"));
        services.Configure<LockoutOptions>(this.Configuration.GetSection("Lockout"));
        services.Configure<HttpLoggingOptions>(this.Configuration.GetSection("HttpLogging"));
        services.Configure<BypassAuthOptions>("Bypass", this.Configuration.GetSection("Auth:Bypass"));
        services.Configure<EmailSettings>(this.Configuration.GetSection("Email"));
        services.Configure<SwaggerOptions>(this.Configuration.GetSection("Swagger"));
        services.Configure<JwtAuthOptions>(JwtBearerDefaults.AuthenticationScheme, this.Configuration.GetSection("Auth:Jwt"), opts =>
        {
            opts.Events = new JwtAuthEvents();
            opts.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Key));
            this.Configuration.GetSection("Auth:Cookies:Default").Bind(opts.RefreshCookie);
            opts.RefreshCookie.MaxAge = opts.RefreshLifetime;
        });
        services
            .Configure<ApiBehaviorOptions>(this.Configuration.GetSection("ApiBehavior"))
            .Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
        ;
        services.Configure<ActivitySettings>(this.Configuration.GetSection("ApiControllers:Activity"));
    }

    protected override void ConfigureIdentity(IServiceCollection services)
    {
        base.ConfigureIdentity(services);
        services.ConfigureSqlDbContext<GmIdentityContext>(this.Configuration);
        services.ConfigureSqlDbContext<CarmaContext>(this.Configuration);
    }
    protected virtual void ConfigureAuth(IServiceCollection services)
    {
        services.AddJwtAuthentication();
        services.AddBypassAuthentication();
        services.AddHttpContextAccessor();
    }
    protected virtual void ConfigureServiceProviders(IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<JwtAuthTokenService, JwtAuthTokenService>();
    }
    protected virtual void ConfigureLogging(IServiceCollection services)
    {
        if (this.HttpLoggingEnabled)
            services.AddHttpLogging(options => { });
    }
    protected virtual void ConfigureFileHosting(IApplicationBuilder app)
    {
        var smap = new Dictionary<string, string>
        {
            { "/Images/UserProfile", this.ProfilePictureRoot }
        };
        app.UseStaticFiles(); // For the wwwroot folder  
        foreach (var sfile in smap)
        {
            Directory.CreateDirectory(sfile.Value);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = sfile.Key,
                FileProvider = new PhysicalFileProvider(sfile.Value)
            });
        }
    }
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        this.ConfigureMvc(services);
        this.ConfigureSettings(services);
        this.ConfigureIdentity(services);
        this.ConfigureAuth(services);
        this.ConfigureServiceProviders(services);
        this.ConfigureLogging(services);
        services.AddControllers();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseForwardedHeaders();
            app.UseHsts();
            if (this.Configuration.GetValue<bool>("EnableDeveloperErrors"))
                app.UseDeveloperExceptionPage();
        }

        if (this.HttpLoggingEnabled)
            app.UseHttpLogging();
        app.UseRouting();

        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        this.ConfigureFileHosting(app);

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();
        if (this.Configuration.GetValue<bool>("Swagger:Enabled", false))
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"[{env.EnvironmentName}] Web API V1");
                c.RoutePrefix = "";
            });
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseHostFiltering();

        MappingFactory.Instance
            .AddProfile<ApiMappingProfile>()
        ;
    }
}
