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

using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Identity.GmIdentity;
using IdentityDbContext = GmWeb.Logic.Data.Context.Identity.GmIdentityContext;

namespace GmWeb.Tests.Api.Data
{
    public class IdentityContextInitializer : ContextInitializer
    {
        protected override string ConnectionName => "Identity";
        public static IdentityContextInitializer Instance { get; private set; }
        public IdentityContextInitializer(
            DataEntities entities,
            CarmaContext commonContext, GmIdentityContext idContext,
            UserManager<GmIdentity> manager,
            IConfiguration configuration,
            IWebHostEnvironment env
        ) : base(entities, commonContext, idContext, manager, configuration, env) { }

        protected (User user, string password) CreateAccount(string email)
        {
            var faker = new Bogus.Faker();
            var password = faker.Internet.Password(10, true, prefix: "pwd-");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                UserName = email,
                LockoutEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.NormalizedEmail = this.Manager.NormalizeEmail(user.Email);
            user.NormalizedUserName = this.Manager.NormalizeName(user.UserName);
            user.PasswordHash = this.Manager.PasswordHasher.HashPassword(user, password);
            this.IdCtx.Users.Add(user);
            return (user, password);
        }
        protected override async Task Populate()
        {
            (this.Entities.AdminIdentity, this.Entities.AdminPassword) = this.CreateAccount(this.Entities.AdminEmail);
            (this.Entities.MemberIdentity, this.Entities.MemberPassword) = this.CreateAccount(this.Entities.MemberEmail);

            await Task.CompletedTask;
        }
        protected override async Task Validate()
        {
            if (this.IdCtx == null)
                throw new NullReferenceException("Cannot get instance of dbContext");

            if (this.ConnectionString.ToLower().Contains("gmidentity") | this.ConnectionString.ToLower().Contains("gmcommon"))
                throw new Exception("LIVE SETTINGS IN TESTS!");

            await this.IdCtx.Database.EnsureDeletedAsync();
            await this.IdCtx.Database.EnsureCreatedAsync();
        }
        protected override async Task<int> SaveChangesAsync()
        {
            return await this.IdCtx.SaveChangesAsync();
        }
        protected override int SaveChanges()
        {
            return this.IdCtx.SaveChanges();
        }
    }
}
