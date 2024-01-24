using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Interfaces;

public interface ICompleteUserStore<TUser>
        : IUserStore<TUser>
        , IUserClaimStore<TUser>
        , IUserEmailStore<TUser>
        , IUserPasswordStore<TUser>
        , IUserPhoneNumberStore<TUser>
        , IUserTwoFactorStore<TUser>
        , IUserAuthenticatorKeyStore<TUser>
        , IUserTwoFactorRecoveryCodeStore<TUser>
        , IUserSecurityStampStore<TUser>
    where TUser : class
{
}
