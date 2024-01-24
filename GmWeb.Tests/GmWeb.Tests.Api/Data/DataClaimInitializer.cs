using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Carma.User;
using Agency = GmWeb.Logic.Data.Models.Carma.Agency;
using ActivityCalendar = GmWeb.Logic.Data.Models.Carma.ActivityCalendar;

using Microsoft.AspNetCore.Identity;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using Gender = GmWeb.Logic.Enums.Gender;
using UserRole = GmWeb.Logic.Enums.UserRole;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using EnumExtensions = GmWeb.Logic.Utility.Extensions.Enums.EnumExtensions;
using System.Diagnostics;

namespace GmWeb.Tests.Api.Data
{
    [Obsolete]
    public class DataClaimInitializer : ContextInitializer
    {
        protected override string ConnectionName => "Identity";
        public static DataClaimInitializer Instance { get; private set; }

        public DataClaimInitializer(
            DataEntities entities,
            CarmaContext commonContext, GmIdentityContext idContext,
            UserManager<GmIdentity> manager,
            IConfiguration configuration,
            IWebHostEnvironment env
        ) : base(entities, commonContext, idContext, manager, configuration, env) { }

        protected override async Task Populate()
        {
            var agencyClaim = new IdentityUserClaim<Guid>
            {
                UserId = this.Entities.AdminIdentity.Id,
                ClaimType = "AgencyId",
                ClaimValue = this.Entities.Agency.AgencyID.ToString()
            };
            this.IdCtx.UserClaims.Add(agencyClaim);

            var accountUsers = await this.ComCtx.Users
                .AsQueryable()
                .Where(x => x.AccountID.HasValue)
                .ToListAsync()
            ;
            foreach(var u in accountUsers)
            {
                var roleClaim = new IdentityUserClaim<Guid>
                {
                    UserId = u.AccountID.Value,
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = u.UserRole.ToString()
                };
                this.IdCtx.UserClaims.Add(roleClaim);
            }
        }
        protected override async Task Validate()
        {
            if (this.IdCtx == null)
                throw new NullReferenceException("Cannot get instance of identity context.");            
            if (this.ComCtx == null)
                throw new NullReferenceException("Cannot get instance of common context");

            if (this.ConnectionString.ToLower().Contains("gmidentity") || this.ConnectionString.ToLower().Contains("gmcommon"))
                throw new Exception("LIVE SETTINGS IN TESTS!");

            await Task.CompletedTask;
        }
        protected override async Task<int> SaveChangesAsync()
        {
            var count = 0;
            count += await this.IdCtx.SaveChangesAsync();
            return count;
        }
        protected override int SaveChanges()
        {
            var count = 0;
            count += this.IdCtx.SaveChanges();
            return count;
        }
    }
}
