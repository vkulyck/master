using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Data.Context.Carma;
using Microsoft.EntityFrameworkCore;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;

namespace GmWeb.Logic.Utility.Identity;
public class GmUserManager : CompleteUserManager
{
    private readonly CarmaCache _carma;

    private readonly GmUserStore _store;
    protected CarmaCache Carma => this._carma;
    new public GmUserStore Store => this._store;

    public GmUserManager(
            CarmaContext context,
            GmUserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            GmPasswordHasher hasher,
            IEnumerable<IUserValidator<GmIdentity>> userValidators,
            IEnumerable<IPasswordValidator<GmIdentity>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<GmIdentity>> logger
        ) : base(store, optionsAccessor, hasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        this._store = store;
        this._carma = new CarmaCache(context);
    }

    public override async Task<GmIdentity> FindByNameAsync(string userName)
    {
        string normName = this.KeyNormalizer.NormalizeName(userName);
        var appUser = await this.Store.FindByNameAsync(normName);
        if (appUser == null)
            return null;
        var cmUser = await this._carma.Users.SingleOrDefaultAsync(x => x.AccountID == appUser.Id);
        if(cmUser != null)
            MappingFactory.Instance.Map(cmUser, appUser);
        return appUser;
    }    
    
    public override async Task<GmIdentity> FindByEmailAsync(string email)
    {
        string normEmail = this.KeyNormalizer.NormalizeEmail(email);
        var appUser = await this.Store.FindByEmailAsync(normEmail);
        if (appUser == null)
            return null;
        var cmUser = await this._carma.Users.SingleOrDefaultAsync(x => x.AccountID == appUser.Id);
        if(cmUser != null)
            MappingFactory.Instance.Map(cmUser, appUser);
        return appUser;
    }

    public override async Task<GmIdentity> FindByIdAsync(string userId)
    {
        var appUser = await this.Store.FindByIdAsync(userId);
        if (appUser == null)
            return null;
        var cmUser = await this._carma.Users.SingleOrDefaultAsync(x => x.AccountID == appUser.Id);
        if (cmUser != null)
            MappingFactory.Instance.Map(cmUser, appUser);
        return appUser;
    }
    public override async Task<IdentityResult> CreateAsync(GmIdentity appUser, string password)
    {
        appUser.PasswordHash = this.PasswordHasher.HashPassword(appUser, password);
        await UpdateNormalizedUserNameAsync(appUser);
        await UpdateNormalizedEmailAsync(appUser);
        var result = await this.Store.CreateAsync(appUser);
        //appUser.AccountID = appUser.Id;
        var cmUser = this._carma.Users.Create(u =>
        {
            MappingFactory.Instance.Map(appUser, u);
        });
        await this._carma.SaveAsync();

        await UpdateSecurityStampAsync(appUser);
        return result;
    }

    public override async Task<IdentityResult> UpdateAsync(GmIdentity appUser)
    {
        var cmUser = await this._carma.Users.SingleOrDefaultAsync(x => x.AccountID == appUser.Id);
        MappingFactory.Instance.Map(appUser, cmUser);
        await this._carma.SaveAsync();
        return await base.UpdateAsync(appUser);
    }

    public override async Task<IdentityResult> DeleteAsync(GmIdentity appUser)
    {
        var cmUser = await this._carma.Users.SingleOrDefaultAsync(x => x.AccountID == appUser.Id);
        MappingFactory.Instance.Map(appUser, cmUser);
        await this._carma.Users.DeleteAsync(cmUser);
        return await base.DeleteAsync(appUser);
    }
}