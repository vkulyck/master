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

namespace GmWeb.Logic.Utility.Identity;
public class CompleteUserManager : UserManager<GmIdentity>
{
    private readonly GmPasswordHasher _hasher;
    new public GmPasswordHasher PasswordHasher => this._hasher;

    public CompleteUserManager(
            ICompleteUserStore<GmIdentity> store,
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
        this._hasher = hasher;
    }
}