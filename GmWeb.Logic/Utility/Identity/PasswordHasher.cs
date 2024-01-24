using GmWeb.Common.Crypto;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Identity
{
    public class GmPasswordHasher : IPasswordHasher<GmIdentity>, IPasswordValidator<GmIdentity>
    {
        public string HashPassword(GmIdentity user, string password)
        {
            string hash = Hasher.HashPassword(password);
            return hash;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<GmIdentity> manager, GmIdentity user, string password)
        {
            var result = await manager.CheckPasswordAsync(user, password);
            if (result)
                return IdentityResult.Success;
            return IdentityResult.Failed(new IdentityError { Description = $"User {user.Id} failed password validation." });
        }

        public PasswordVerificationResult VerifyHashedPassword(GmIdentity user, string hashedPassword, string providedPassword)
        {
            bool isValid = Hasher.ValidatePassword(providedPassword, hashedPassword);
            if (isValid)
            {
                if (Base64.IsBase64String(hashedPassword))
                    return PasswordVerificationResult.Success;
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            return PasswordVerificationResult.Failed;
        }
    }
}