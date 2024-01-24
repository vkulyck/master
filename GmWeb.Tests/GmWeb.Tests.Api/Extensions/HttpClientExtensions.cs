using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TokenModel = GmWeb.Web.Common.Auth.Tokens.TokenResult;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Extensions.Http;

namespace GmWeb.Tests.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static HttpClient Authorize(this HttpClient client, JwtAuthToken token)
            => client.Authorize(token.Serialize());
        public static HttpClient Authorize(this HttpClient client, TokenModel token)
            => client.Authorize(token.AuthToken);
        public static HttpClient Authorize(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            return client;
        }
    }
}
