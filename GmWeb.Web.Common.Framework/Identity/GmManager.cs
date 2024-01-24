using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using GmWeb.Web.Common.Utility;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Security.DataProtection;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Context.Identity;
using GmWeb.Common;
using System.Configuration;
using Newtonsoft.Json;
using ConfigAccessor = GmWeb.Logic.Utility.Config.ConfigAccessor;
using EmailSettings = GmWeb.Logic.Utility.Email.EmailSettings;

namespace GmWeb.Web.Common.Identity
{
    public class GmManager : GmManager<GmIdentity>
    {
        public GmManager(IUserStore<GmIdentity> store, AccountCache cache, IdentityDbContext<GmIdentity> dbContext, IOwinContext owinContext) 
            : base(store, cache, dbContext, owinContext) 
        { }

        public static GmManager Create(IdentityFactoryOptions<GmManager> options, IOwinContext owinContext)
        {
            var dbContext = owinContext.Get<GmIdentityContext>();
            var accountCache = owinContext.Get<AccountCache>();
            var store = new GmStore(dbContext, accountCache);
            var manager = new GmManager(store, accountCache, dbContext, owinContext);
            return manager;
        }
    }
    public class GmManager<TUser> : ApplicationUserManager<TUser, GmStore<TUser>, GmManager<TUser>>
        where TUser : GmIdentity, new()
    {
        public GmManager(IUserStore<TUser> store, AccountCache cache, IdentityDbContext<TUser> dbContext, IOwinContext owinContext)
            : base(store, cache, dbContext, owinContext)
        { }
    }

    public abstract class ApplicationUserManager<TUser, TUserStore, TManager> : UserManager<TUser>, ITwoFactorManager<TUser>
        where TUser : GmIdentity, new()
        where TUserStore : GmStore<TUser>
        where TManager : ApplicationUserManager<TUser, TUserStore, TManager>
    {
        protected IOwinContext OwinContext { get; private set; }
        protected IAuthenticationManager AuthenticationManager => this.OwinContext.Authentication;
        protected IDataProtectionProvider ProtectionProvider { get; } = new DpapiDataProtectionProvider("GmWeb");
        protected IDataProtector Protector { get; set; }
        protected IdentityDbContext<TUser> Context { get; set; }
        protected AccountCache Cache { get; set; }
        private IEnumerable<InformationType> _EnabledTwoFactorTypes;
        public IEnumerable<InformationType> EnabledTwoFactorTypes
        {
            get
            {
                if (_EnabledTwoFactorTypes == null)
                {
                    var config = ConfigurationManager.AppSettings["2FA.EnabledTwoFactorTypes"]?.ToString();
                    if (EnumExtensions.TryParse(config, out List<InformationType> results, ';', ',', ':'))
                        _EnabledTwoFactorTypes = results;
                    else
                        throw new ArgumentException($"Error parsing 2FA information types.");
                }
                return _EnabledTwoFactorTypes;
            }
        }

        public IDictionary<InformationType, TotpSecurityStampBasedTokenProvider<TUser, string>> TwoFactorMethods { get; }
            = new Dictionary<InformationType, TotpSecurityStampBasedTokenProvider<TUser, string>>();

        public TManager WithAppIdentityType(AppIdentityType appIdentityType)
        {
            var gmStore = this.Store as GmStore;
            gmStore.AppIdentityType = appIdentityType;
            return (TManager)this;
        }

        public ApplicationUserManager(IUserStore<TUser> store, AccountCache cache, IdentityDbContext<TUser> dbContext, IOwinContext owinContext)
            : base(store)
        {
            this.PasswordHasher = new PasswordHasher();
            this.Protector = this.ProtectionProvider.Create("GmWebToken");
            this.UserTokenProvider = new DataProtectorTokenProvider<TUser>(this.Protector);

            // Lockout
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            foreach (var infoType in this.EnabledTwoFactorTypes)
            {
                var provider = TwoFactorProviderFactory<TUser>.Create(infoType);
                this.RegisterTwoFactorProvider(infoType, provider);
            }
#if AUTOLOGIN
            var autoProvider = new GmWeb.Web.Common.Identity.DevAutoLoginProvider<TUser>();
            this.RegisterTwoFactorProvider(InformationType.DevAutoLogin, autoProvider);
#endif
            this.SmsService = owinContext.Get<SmsService>();
            this.EmailService = owinContext.Get<EmailService>();
            this.UserValidator = new UserValidator<TUser>(this);
            this.ClaimsIdentityFactory = new ClaimsIdentityFactory<TUser>();

            this.Context = dbContext;
            this.Cache = cache;
            this.OwinContext = owinContext;
        }

        public void RegisterTwoFactorProvider(InformationType infoType, TotpSecurityStampBasedTokenProvider<TUser, string> provider)
        {
            this.TwoFactorMethods[infoType] = provider;
            this.RegisterTwoFactorProvider(infoType.ToString(), provider);
        }

        public override async Task<IList<string>> GetRolesAsync(string userId)
        {
            var user = await this.Store.FindByIdAsync(userId);
            var roleIds = await user.Roles.Select(x => x.RoleId).ToAsyncEnumerable().ToList();
            var roles = await this.Context.Roles.Where(x => roleIds.Contains(x.Id)).ToAsyncEnumerable().ToList();
            var result = await roles.Select(x => x.Name).ToAsyncEnumerable().ToList();
            return result;
        }

        public override async Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            var user = await this.Store.FindByIdAsync(userId);
            var claims = await user.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToAsyncEnumerable().ToList();
            return claims;
        }

        public async Task<IDictionary<InformationType, bool>> GetProviderConfigurationAsync(TUser user)
        {
            var map = new Dictionary<InformationType, bool>();
            foreach (var kvp in this.TwoFactorMethods)
            {
                var isEnabled = await this.GetUserTfaEnabledAsync(user, kvp.Key);
                map.Add(kvp.Key, isEnabled);
            }
            return map;
        }

        public override async Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            var uidClaim = new IdentityUserClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                UserId = userId
            };
            var user = await this.Store.FindByIdAsync(userId);
            user.Claims.Add(uidClaim);
            await this.Context.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public override async Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim)
        {
            var user = await this.Store.FindByIdAsync(userId);
            var userClaims = this.Context.Set<IdentityUserClaim>();
            var matchedClaim = await user.Claims.Where(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value).ToAsyncEnumerable().SingleOrDefault();
            userClaims.Remove(matchedClaim);
            await this.Context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public override async Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId)
        {
            var user = await this.Store.FindByIdAsync(userId);
            List<string> providers = new List<string>();
            foreach (var infoType in this.TwoFactorMethods.Keys)
            {
                var isEnabled = await this.GetUserTfaEnabledAsync(user, infoType);
                if (!isEnabled)
                    continue;
                providers.Add(infoType.ToString());
            }
            return providers;
        }

        public override async Task<bool> GetTwoFactorEnabledAsync(string userId)
        {
            return (await this.GetValidTwoFactorProvidersAsync(userId)).Any();
        }

        public async Task<string> ConfigureTwoFactorSecret(string userId, string providerId, InformationType infoType)
        {
            var provider = this.GetTfaProvider(infoType) as ISharedTokenGeneratorProvider<TUser>;
            if (provider == null)
                throw new ArgumentException($"The specified provider does not implement symmetric token generation.");
            var user = await this.Store.FindByIdAsync(userId);
            var secretKey = await provider.ConfigureSharedSecret(this, user);
            return secretKey;
        }
    }
}
