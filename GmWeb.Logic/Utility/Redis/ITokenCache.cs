using System;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Redis
{

    public partial interface ITokenCache : IDisposable
    {
        Task<bool> RegisterAuthorizationToken(string userId, string token, TimeSpan expiresIn);
        Task<bool> RevokeAuthorizationToken(string userId, string token);
        Task<bool> ValidateAuthorizationToken(string userId, string token);
        Task<string> GenerateRegistrationTokenAsync(string email);
        Task<string> GenerateRegistrationTokenAsync(UserRegistrationData data);
        Task<TData> GetRegistrationDataAsync<TData>(string token) where TData : UserRegistrationData;
        Task<VerifyRegistrationResponse> VerifyRegistrationTokenAsync(string token);
    }
}
