using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Context.Identity;
using GmWeb.Common;
using GmWeb.Web.Common.Utility;
using System.Security.Claims;

namespace GmWeb.Web.Common.Identity
{
    public class GmStore :  GmStore<GmIdentity>
    {
        public GmStore(GmIdentityContext context, AccountCache accountCache)
            : base(context, accountCache)
        { }
    }
    public class GmStore<TUser> : GmStore<TUser, IdentityDbContext<TUser>>
        where TUser : GmIdentity
    {
        public GmStore(IdentityDbContext<TUser> context, AccountCache accountCache)
            : base(context, accountCache)
        { }
    }
    public class GmStore<TUser,TContext> : UserStore<TUser>
        where TUser : GmIdentity
        where TContext : IdentityDbContext<TUser>
    {
        public TContext IdentityContext => (TContext)this.Context;
        public AccountCache AccountCache { get; private set; }
        public AppIdentityType AppIdentityType { get; set; }
        public GmStore(TContext context, AccountCache accountCache) : base(context)
        {
            this.AccountCache = accountCache;
        }

        protected async Task<BaseAccount> GetSourceAccountAsync(Expression<Func<BaseAccount, bool>> predicate)
        {
            IQueryable<BaseAccount> query;
            switch(this.AppIdentityType)
            {
                case AppIdentityType.User:
                    query = this.AccountCache.UserAccounts.Cast<BaseAccount>();
                    break;
                case AppIdentityType.Client:
                case AppIdentityType.Agency:
                    query = this.AccountCache.ClientAccounts.Cast<BaseAccount>();
                    break;
                default:
                    throw new NotImplementedException();
            }
            var queryTask = query.Where(predicate).ToAsyncEnumerable().SingleOrDefault();
            var model = await queryTask;
            if (model == null)
                throw new NullReferenceException();
            return model;
        }

        public override async Task<TUser> FindByIdAsync(string userId)
        {
            //var identity = await this.Context.Users.Where(x => x.Id == userId).ToAsyncEnumerable().SingleOrDefault();
            var identity = this.IdentityContext.Users.Where(x => x.Id == userId).SingleOrDefault();
            if (identity == null)
                return null;
            var source = await this.GetSourceAccountAsync(x => x.Email == identity.Email);
            identity.Account = source;
            return identity;
        }

        public override Task<TUser> FindAsync(UserLoginInfo login)
            => throw new NotImplementedException();

        public override async Task<TUser> FindByNameAsync(string userName)
        {
            var identity = await this.IdentityContext.Users.Where(x => x.UserName == userName).ToAsyncEnumerable().SingleOrDefault();
            if (identity == null)
                return null;
            var source = await this.GetSourceAccountAsync(x => x.Email == identity.Email);
            identity.Account = source;
            return identity;
        }

        public override async Task CreateAsync(TUser identity)
        {
            this.IdentityContext.Users.Add(identity);
            await this.Context.SaveChangesAsync();

            using (var context = new AccountContext())
            {
                var userExists = context.UserAccounts.Any(x => x.Email == identity.Email);
                if (!userExists)
                {
                    var userResponse = await context.LegacyRegistration.CreateUser(identity.Email, identity.PasswordHash);
                    if (userResponse != LegacyRegistration.ResponseCode.Success)
                        return; // Failure
                }
                if (identity.IsUser())
                    return; // Success
                var clientExists = context.ClientAccounts.Any(x => x.Email == identity.Email);
                if(!clientExists)
                {
                    var clientResponse = await context.LegacyRegistration.CreateClient(identity.Email, identity.PasswordHash);
                    if (clientResponse != LegacyRegistration.ResponseCode.Success)
                        return; // Failure
                }
            } // Success
        }

        public override async Task UpdateAsync(TUser identity)
        {
            var entry = this.Context.Entry(identity);
            var changes = this.Context.ChangeTracker.Entries().ToList();
            foreach(var change in changes)
            {
                switch(change.State)
                {
                    case System.Data.Entity.EntityState.Added:
                        Console.WriteLine($"{change.State}:", change);
                        break;
                    case System.Data.Entity.EntityState.Modified:
                        Console.WriteLine($"{change.State}:", change);
                        break;
                    case System.Data.Entity.EntityState.Deleted:
                        Console.WriteLine($"{change.State}:", change);
                        break;
                    case System.Data.Entity.EntityState.Detached:
                        Console.WriteLine($"{change.State}:", change);
                        break;
                    case System.Data.Entity.EntityState.Unchanged:
                        Console.WriteLine($"{change.State}:", change);
                        break;
                }
            }
            entry.State = System.Data.Entity.EntityState.Modified;
            await this.Context.SaveChangesAsync();

            if (identity.Account == null)
                return;
            switch (this.AppIdentityType)
            {
                case AppIdentityType.User:
                    await this.AccountCache.UserAccounts.UpdateSingleAsync(identity.Account);
                    return;
                case AppIdentityType.Client:
                    await this.AccountCache.ClientAccounts.UpdateSingleAsync(identity.Account);
                    return;
            }
            throw new NotImplementedException();
        }

        public override Task AddClaimAsync(TUser user, Claim claim)
        {
            return base.AddClaimAsync(user, claim);
        }
    }
}
