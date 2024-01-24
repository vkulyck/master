using GmWeb.Web.Api.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmRole = GmWeb.Logic.Data.Models.Identity.GmRole;
using UserPasswordInsertDTO = GmWeb.Web.Common.Models.Carma.UserPasswordInsertDTO;

namespace GmWeb.Web.Api.Controllers.Identity;

public class AccountController : GmApiController
{
    private readonly RoleManager<GmRole> _roleManager;

    public AccountController(UserManager<GmIdentity> userManager, RoleManager<GmRole> roleManager)
        : base(userManager)
    {
        this._roleManager = roleManager;
    }

    /// <summary>
    /// Get a user
    /// </summary>
    /// <param name="Id">The identity account ID; equivalent to AccountID.</param>
    /// <param name="AccountID">The identity account ID; equivalent to Id.</param>
    /// <param name="Email">The identity account's email address.</param>
    /// <param name="UserName">The identity account's username.</param>
    /// <returns></returns>
    [HttpGet]
    [ActionName("")]
    [ProducesResponseType(typeof(GmIdentity), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public IActionResult Get([FromQuery] Guid? Id, [FromQuery] Guid? AccountID, [FromQuery] string Email, [FromQuery] string UserName)
    {
        Id = Id ?? AccountID;
        var users = this.Manager.Users;
        if (Id.HasValue)
            users = users.Where(x => x.Id == Id.Value);
        else if (!string.IsNullOrWhiteSpace(Email))
            users = users.Where(x => x.Email == Email || x.NormalizedEmail == Email);
        else if (!string.IsNullOrWhiteSpace(UserName))
            users = users.Where(x => x.UserName == UserName || x.NormalizedUserName == UserName);
        else
            return this.MissingQueryFilters();

        var user = users.SingleOrDefault();
        if (user == null)
            return this.BadRequest("No user accounts found matching search parameters.");

        return this.Ok(user);
    }

    /// <summary>
    /// Insert a user with an existing role
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(GmIdentity), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> InsertWithRole([FromBody] UserPasswordInsertDTO model)
    {
        if (model == null)
            return this.BadRequest("No data in model!");

        if (!this.ModelState.IsValid)
            return this.BadRequest(this.ModelState.Values.Select(x => x.Errors.FirstOrDefault().ErrorMessage));

        var user = new GmIdentity
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = model.EmailConfirmed,
            PhoneNumber = model.PhoneNumber
        };

        var role = await this._roleManager.FindByIdAsync(model.RoleId).ConfigureAwait(false);
        if (role == null)
            return this.BadRequest("Could not find role!");

        var result = await this.Manager.CreateAsync(user, model.Password).ConfigureAwait(false);
        if (result.Succeeded)
        {
            var result2 = await this.Manager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);
            if (result2.Succeeded)
            {
                return this.Ok(new
                {
                    user.Id,
                    user.Email,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    user.LockoutEnabled,
                    user.TwoFactorEnabled
                });
            }
        }
        return this.BadRequest(result.Errors.Select(x => x.Description));
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IdentityResult), 400)]
    [ActionName("update/{Id}")]
    public async Task<ObjectResult> Put([FromRoute]string Id, [FromBody] EditUserViewModel model)
    {
        if (model == null)
            return this.BadRequest("No data in model!").AsIdentityResult();

        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var user = await this.Manager.FindByIdAsync(Id).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!");

        // Add more fields to update
        user.Email = model.Email;
        user.UserName = model.Email;
        user.EmailConfirmed = model.EmailConfirmed;
        user.PhoneNumber = model.PhoneNumber;
        user.LockoutEnabled = model.LockoutEnabled;
        user.TwoFactorEnabled = model.TwoFactorEnabled;

        var result = await this.Manager.UpdateAsync(user).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return this.Success(result);
        }
        return this.IdentityErrors(result);
    }

    /// <summary>
    /// Delete a user (Will also delete link to roles)
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
            return this.BadRequest("Empty parameter!").AsIdentityResult();

        var user = await this.Manager.FindByIdAsync(Id).ConfigureAwait(false);
        if (user == null)
            return this.BadRequest("Could not find user!").AsIdentityResult();

        var result = await this.Manager.DeleteAsync(user).ConfigureAwait(false);
        return this.Infer(result);
    }
}
