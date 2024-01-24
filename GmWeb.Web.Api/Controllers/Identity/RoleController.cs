
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmRole = GmWeb.Logic.Data.Models.Identity.GmRole;

namespace GmWeb.Web.Api.Controllers.Identity;

public class RoleController : GmApiController
{
    private readonly RoleManager<GmRole> _roleManager;

    public RoleController(RoleManager<GmRole> roleManager, UserManager<GmIdentity> userManager)
        : base(userManager)
    {
        this._roleManager = roleManager;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IdentityRole>), 200)]
    public IActionResult Get() => this.Ok(
        this._roleManager.Roles
        .Select(role => new
        {
            role.Id,
            role.Name
        }));

    /// <summary>
    /// Insert a role
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    public async Task<IActionResult> Insert([FromBody] RoleViewModel model)
    {
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors().AsIdentityResult();

        var identityRole = new GmRole { Name = model.Name };

        var result = await this._roleManager.CreateAsync(identityRole).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Ok(new
            {
                identityRole.Id,
                identityRole.Name
            });
        }
        return this.BadRequest(result);
    }

    /// <summary>
    /// Update a role
    /// </summary>
    /// <param name="Id">Role Id</param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [ActionName("update/{Id}")]
    public async Task<IActionResult> Put([FromRoute] string Id, [FromBody] RoleViewModel model)
    {
        if (model == null)
            return this.ModelStateErrors().AsIdentityResult();

        var identityRole = await this._roleManager.FindByIdAsync(Id).ConfigureAwait(false);

        identityRole.Name = model.Name;

        var result = await this._roleManager.UpdateAsync(identityRole).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Ok();
        }
        return this.BadRequest(result);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [ActionName("delete/{Id}")]
    public async Task<IActionResult> Delete([FromRoute]string Id)
    {
        if (string.IsNullOrEmpty(Id))
            return this.MissingRouteId().AsIdentityResult();

        var identityRole = await this._roleManager.FindByIdAsync(Id).ConfigureAwait(false);
        if (identityRole == null)
            return this.DataNotFound().AsIdentityResult();

        var result = await this._roleManager.DeleteAsync(identityRole).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Ok();
        }
        return this.BadRequest(result);
    }
}
