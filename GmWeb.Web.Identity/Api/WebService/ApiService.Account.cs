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

namespace GmWeb.Web.Identity.Api.WebService;
public partial class ApiService
{
    public async Task<GmIdentity> FindUserByIdAsync(string AccountID) 
        => await this.GetAsync<GmIdentity>($"account", data: new { AccountID });
    public async Task<GmIdentity> FindUserByUsernameAsync(string UserName)
        => await this.GetAsync<GmIdentity>($"account", data: new { UserName });
    public async Task<GmIdentity> FindUserByEmailAsync(string Email)
        => await this.GetAsync<GmIdentity>($"account", data: new { Email });

    public async Task<IdentityResult> UpdateUserAsync(GmIdentity user)
    {
        return await this.PostAsync<IdentityResult>($"account/update/{user.AccountID}", user);
    }

    public async Task<GmIdentity> GetUser(Guid AccountID)
    {
        var user = await this.GetAsync<GmIdentity>($"account/id/{AccountID}");
        return user;
    }
}