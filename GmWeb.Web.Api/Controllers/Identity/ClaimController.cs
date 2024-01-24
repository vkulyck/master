using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.UserManager;

namespace GmWeb.Web.Api.Controllers.Identity;

public partial class ClaimController : GmApiController
{
    public ClaimController(UserManager<GmIdentity> manager) : base(manager) { }

    [HttpGet]
    [ProducesResponseType(typeof(List<ClaimDTO>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> GetClaims(Guid AccountID, string ClaimType = null)
    {
        var user = await this.Manager.FindByIdAsync(AccountID.ToString());
        var claims = await this.Manager.GetClaimsAsync(user);
        if (!string.IsNullOrWhiteSpace(ClaimType))
            claims = claims.Where(x => x.Type == ClaimType).ToList();
        var mapped = this.Mapper.Map<ClaimDTO>(claims);
        return this.Ok(mapped);
    }

    public struct AddClaimsDTO
    {
        public Guid AccountID { get; set; }
        public List<ClaimDTO> Claims { get; set; }
    }
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> AddClaims(AddClaimsDTO dto)
    {
        var user = await this.Manager.FindByIdAsync(dto.AccountID);
        var result = await this.Manager.AddClaimsAsync(user, dto.Claims);
        if (result.Succeeded)
            return this.Ok(result);
        return this.BadRequest(result);
    }

    public struct ReplaceClaimDTO
    {
        public Guid AccountID { get; set; }
        public ClaimDTO Claim { get; set; }
        public ClaimDTO NewClaim { get; set; }
    }
    [HttpPut]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> ReplaceClaim(ReplaceClaimDTO dto)
    {
        var user = await this.Manager.FindByIdAsync(dto.AccountID);
        var result = await this.Manager.ReplaceClaimAsync(user, dto.Claim, dto.NewClaim);
        if (result.Succeeded)
            return this.Ok(result);
        return this.BadRequest(result);
    }

    public struct RemoveClaimsDTO
    {
        public Guid AccountID { get; set; }
        public List<ClaimDTO> Claims { get; set; }
    }
    [HttpDelete]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> RemoveClaims(RemoveClaimsDTO dto)
    {
        var user = await this.Manager.FindByIdAsync(dto.AccountID);
        var result = await this.Manager.RemoveClaimsAsync(user, dto.Claims);
        if (result.Succeeded)
            return this.Ok(result);
        return this.BadRequest(result);
    }
    [HttpGet]
    [ProducesResponseType(typeof(List<GmIdentity>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> GetUsersForClaim(ClaimDTO claim)
    {
        var users = await this.Manager.GetUsersForClaimAsync(claim);
        return this.Ok(users);
    }
}
