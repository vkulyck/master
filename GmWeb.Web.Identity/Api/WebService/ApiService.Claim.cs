using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<IList<Claim>> GetClaimsAsync(GmIdentity user, CancellationToken cancellationToken)
    {
        var dtoClaims = await this.GetAsync<List<ClaimDTO>>("claim/get-claims", new { AccountID = user.AccountID });
        var claims = this.Mapper.Map<Claim>(dtoClaims).ToList();
        return claims;
    }

    public async Task<IdentityResult> AddClaimsAsync(GmIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken) 
        => await this.PostAsync<IdentityResult>("claim/add-claims", new { AccountID = user.AccountID, Claims = claims });

    public async Task<IdentityResult> ReplaceClaimAsync(GmIdentity user, Claim claim, Claim newClaim, CancellationToken cancellationToken) 
        => await this.PutAsync<IdentityResult>("claim/replace-claim", new { AccountID = user.AccountID, claim, newClaim });

    public async Task<IdentityResult> RemoveClaimsAsync(GmIdentity user, IEnumerable<Claim> claims, CancellationToken cancellationToken) 
        => await this.DeleteAsync<IdentityResult>("claim/remove-claims", new { AccountID = user.AccountID, claims });

    public async Task<IList<GmIdentity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) 
        => await this.GetAsync<List<GmIdentity>>("claim/get-users-for-claim", claim);
}