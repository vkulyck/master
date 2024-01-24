using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmWeb.Web.Scheduler.Extensions;
using Newtonsoft.Json.Serialization;

namespace GmWeb.Web.Scheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Add MVC services to the services container.
            services
                .AddMvc()
                .AddSessionStateTempDataProvider()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
                

            services
                .AddDistributedMemoryCache()
                .AddSession(opts => {
                    opts.Cookie.IsEssential = true;
                });

            // Add Kendo UI services to the services container
            services.AddKendo();

            // Add Demo database services to the services container
            services.AddKendoDemo();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CompositeFileProvider(
                    env.ContentRootFileProvider,
                    new ManifestEmbeddedFileProvider(typeof(Startup).Assembly, "embedded")
                )
            });

            app.UseSession();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Scheduler}/{action=Index}/{id?}");
            });
        }
    }
}
