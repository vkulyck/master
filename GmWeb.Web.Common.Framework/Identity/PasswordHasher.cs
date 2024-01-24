using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using GmWeb.Common.Crypto;

namespace GmWeb.Web.Common.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            var hash = Hasher.HashPassword(password);
            return hash;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var isValid = Hasher.ValidatePassword(providedPassword, hashedPassword);
            if (isValid)
            {
                if (IsBase64String(hashedPassword))
                    return PasswordVerificationResult.Success;
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            return PasswordVerificationResult.Failed;
        }

        public static bool IsBase64String(string base64)
        {
            if (base64.Length % 4 != 0)
                return false;
            if (!Regex.IsMatch(base64, @"^[A-Za-z0-9\+\/]+={0,2}$"))
                return false;
            return true;
        }
    }
}