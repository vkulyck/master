using BaseSignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GmWeb.Logic.Utility.Identity.Results;

public class MutableSignInResult
{
    public bool Succeeded { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsNotAllowed { get; set; }
    public bool RequiresTwoFactor { get; set; }

    public MutableSignInResult() { }
    public MutableSignInResult(BaseSignInResult baseResult)
    {
        this.Succeeded = baseResult.Succeeded;
        this.IsLockedOut = baseResult.IsLockedOut;
        this.IsNotAllowed = baseResult.IsNotAllowed;
        this.RequiresTwoFactor = baseResult.RequiresTwoFactor;
    }
    public static implicit operator BaseSignInResult(MutableSignInResult mresult)
    {
        if (mresult.IsLockedOut)
            return BaseSignInResult.LockedOut;
        if (mresult.IsNotAllowed)
            return BaseSignInResult.NotAllowed;
        if (mresult.RequiresTwoFactor)
            return BaseSignInResult.TwoFactorRequired;
        if (mresult.Succeeded)
            return BaseSignInResult.Success;
        return BaseSignInResult.Failed;
    }
}