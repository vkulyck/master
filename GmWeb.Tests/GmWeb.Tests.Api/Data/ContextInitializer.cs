using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Bogus;
using GmWeb.Logic.Data.Context.Carma;

namespace GmWeb.Tests.Api.Data
{
    public abstract class ContextInitializer
    {
        protected DataEntities Entities { get; private set; }
        protected CarmaContext ComCtx { get; private set; }
        protected GmIdentityContext IdCtx { get; private set; }
        protected UserManager<GmIdentity> Manager { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected IWebHostEnvironment Env { get; private set; }
        protected string ConnectionString => this.Configuration.GetConnectionString(this.ConnectionName);
        protected abstract string ConnectionName { get; }

        public ContextInitializer(
            DataEntities entities, 
            CarmaContext commonContext, GmIdentityContext idContext, 
            UserManager<GmIdentity> manager,
            IConfiguration configuration,
            IWebHostEnvironment env
        )
        {
            this.Entities = entities;
            this.ComCtx = commonContext;
            this.IdCtx = idContext;
            this.Manager = manager;
            this.Configuration = configuration;
            this.Env = env;
        }

        protected abstract Task Populate();
        protected abstract Task Validate();
        protected abstract Task<int> SaveChangesAsync();
        protected abstract int SaveChanges();

        public virtual async Task Initialize()
        {
            await this.Validate();
            await this.Populate();
            await this.SaveChangesAsync();
        }
    }
}
