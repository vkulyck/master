using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace GmWeb.Web.Common.Utility
{
    public static class TwoFactorClaimTypes
    {
        public static readonly string TokenProviderSecret = "TokenProviderSecret";
        public static readonly string TokenProviderEnabled = "TokenProviderEnabled";
    }
    public enum ClaimTypeElement { Major = 0, Minor = 1, Detail = 2}
    public static class ClaimExtensions
    {
        #region Claim Types
        public static string GetProviderKey<TUser>(this IUserTokenProvider<TUser, string> provider)
            where TUser : class, IUser<string>
        {
            var className = provider.GetType().Name;
            var providerKey = Regex.Replace(className, @"^([A-Z]\w+)TokenProvider.*$", "$1");
            if (string.IsNullOrWhiteSpace(providerKey) || providerKey == className)
                throw new ArgumentException("Token providers must follow the naming convention of '{ProviderKey}TokenProvider'");
            return providerKey;
        }
        public static string[] TypeElements(this Claim claim) => claim.Type.Split(';').ToArray();
        public static string TypeElement(this Claim claim, ClaimTypeElement element)
            => claim.TypeElement((int)element);
        public static string TypeElement(this Claim claim, int index)
        {
            var elements = claim.TypeElements();
            if (elements.Length >= index)
                return null;
            return elements[index];
        }
        public static string MajorType(this Claim claim) => claim.TypeElement(ClaimTypeElement.Major);
        public static string MinorType(this Claim claim) => claim.TypeElement(ClaimTypeElement.Minor);
        public static string Detail(this Claim claim) => claim.TypeElement(ClaimTypeElement.Detail);
        public static async Task<Claim> FindAsync(this IEnumerable<Claim> claims, string majorType, string minorType, string detail)
            => await claims.FindAsync(string.Join(";", majorType, minorType, detail));
        public static async Task<Claim> FindAsync(this IEnumerable<Claim> claims, string majorType, string minorType)
            => await claims.FindAsync(string.Join(";", majorType, minorType));
        public static async Task<Claim> FindAsync(this IEnumerable<Claim> claims, string type)
            => await claims
            .Where(x => x.Type == string.Join(";", type))
            .ToAsyncEnumerable()
            .SingleOrDefault()
        ;

        #endregion
    }
}