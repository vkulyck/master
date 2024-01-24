using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthHeader = System.Net.Http.Headers.AuthenticationHeaderValue;
using HttpMethod = System.Net.Http.HttpMethod;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using StatusCode = System.Net.HttpStatusCode;
using ApiIdentityResult = GmWeb.Logic.Utility.Identity.Results.ApiIdentityResult;

namespace GmWeb.Web.Common.Auth.Services.Passport;
public abstract partial class PassportService
{
    public virtual async Task<HttpClient> CreateHttpClient(bool refresh)
    {
        if (this.Passport == null)
            return null;
        bool expiresSoon = (this.Passport.Expiration <= DateTime.Now.AddMinutes(5));
        if (!refresh && expiresSoon)
        {
            var renewed = await this.RenewPassportAsync();
            if (renewed == null)
                return null;
        }
        if (this.Passport == null)
            return null;

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthHeader(JwtBearerDefaults.AuthenticationScheme, this.Passport.AuthToken);
        httpClient.DefaultRequestHeaders.Add(JwtRefreshToken.DataSourceKey, this.Passport.RefreshToken);
        return httpClient;
    }
}
