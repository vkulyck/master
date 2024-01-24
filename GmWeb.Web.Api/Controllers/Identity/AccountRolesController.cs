using GmWeb.Web.Api.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.UserManager;
using GmRole = GmWeb.Logic.Data.Models.Identity.GmRole;

namespace GmWeb.Web.Api.Controllers.Identity;

public class AccountRolesController : GmApiController
{
    private readonly RoleManager<GmRole> _roleManager;

    public AccountRolesController(UserManager<GmIdentity> userManager, RoleManager<GmRole> roleManager)
        : base(userManager)
    {
        this._roleManager = roleManager;
    }

    /// <summary>
    /// Get a user roles
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpGet("{Id}")]
    [ProducesResponseType(typeof(IEnumerable<string>), 200)]
    public async Task<IActionResult> Get(string Id)
    {
        var user = await this.Manager.FindByIdAsync(Id).ConfigureAwait(false);
        return this.Ok(await this.Manager.GetRolesAsync(user).ConfigureAwait(false));
    }

    /// <summary>
    /// Add a user to existing role
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> Add([FromBody] GmIdentity User, string RoleId)
    {
        if (User == null)
            return this.BadRequest("No data in model!").AsIdentityResult();

        if (!this.ModelState.IsValid)
            return this.BadRequest(this.ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

        var user = await this.Manager.FindByIdAsync(User.AccountID).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!").AsIdentityResult();

        var role = await this._roleManager.FindByIdAsync(RoleId).ConfigureAwait(false);
        if (role == null)
            return this.BadRequest("Could not find role!").AsIdentityResult();

        var result = await this.Manager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Ok(role.Name);
        }
        return this.BadRequest(result);
    }

    /// <summary>
    /// Delete a user from an existing role
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="RoleId"></param>
    /// <returns></returns>
    [HttpDelete("{Id}/{RoleId}")]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> Delete(string Id, string RoleId)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(Id).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!").AsIdentityResult();

        var role = await this._roleManager.FindByIdAsync(RoleId).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find role!").AsIdentityResult();

        var result = await this.Manager.RemoveFromRoleAsync(user, role.Name).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Ok();
        }
        return this.BadRequest(result);
    }
}
