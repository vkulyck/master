using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Identity.DTO;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Logic.Utility.Primitives;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Utility.Attributes;
using GmWeb.Web.Api.Utility;
using GmWeb.Web.Common.Models.Carma;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace GmWeb.Web.Api.Controllers.Common;

/// <summary>
/// The primary CRUD/view controller for Client users.
/// </summary>
public class UserController : CarmaController, IDisposable
{
    public UserController(CarmaContext context, UserManager<GmIdentity> manager, IWebHostEnvironment env) : base(context, manager, env) { }

    /// <summary>
    /// Get the specified user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(UserDTO))]
    [UseBadRequestModel]
    public async Task<IActionResult> Current()
    {
        var account = await this.Manager.GetUserAsync(this.User).ConfigureAwait(false);
        var user = await this.Cache.Users
            .Where(x => x.AccountID == account.Id)
            .SingleOrDefaultAsync()
        ;
        var dto = this.Mapper.Map<UserDTO>(user);
        return this.Ok(dto);
    }
    /// <summary>
    /// Get the specified user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(UserDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    [ActionName("")]
    public virtual async Task<IActionResult> Get([FromQuery] LookupUserDTO lookup)
    {
        var user = await this.Cache.Users.LookupUserAsync(lookup);
        if (user == null)
            return this.DataNotFound();
        var dto = this.Mapper.Map<UserDTO>(user);
        return this.Ok(dto);
    }

    /// <summary>
    /// Get the list of clients
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(ExtendedPagedList<UserDTO, string>))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public virtual async Task<IActionResult> List([FromQuery] ClientListRequest request)
    {
        var agencyError = await this.ResolveRequestAgency(request).ConfigureAwait(false);
        if (agencyError != null)
            return agencyError;
        request.Viewer = await this.GetCarmaUser(request.AgencyID);
        if (request.Viewer == null)
            return this.NotFound();
        var clients = this.Cache.Users.GetAgencyClients(request.AgencyID.Value);
        var extendedRequest = request.Extend();
        var paged = await clients.PageAsync(extendedRequest);
        paged.Items.ForEach(x => x.LoadParentConfig(request.Viewer));
        var mapped = paged.Map<UserDTO>();
        var result = mapped.ToResult();
        return result;
    }

    /// <summary>
    /// Insert an client
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    [HttpPost]
    [UseSuccessModel(typeof(UserDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public async Task<IActionResult> Insert([FromBody] UserInsertDTO client)
    {
        if (client == null)
            return this.MissingPostModel();

        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var entity = this.Mapper.Map<User>(client);

        var inserted = this.Cache.Users.Insert(entity);
        await this.Cache.SaveAsync().ConfigureAwait(false);
        var mapped = this.Mapper.Map<UserDTO>(inserted);
        return this.Ok(mapped);
    }

    /// <summary>
    /// Update an client
    /// </summary>
    /// <param name="requestDTO"></param>
    /// <returns></returns>
    [HttpPut]
    [UseSuccessModel(typeof(UserDTO))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public async Task<IActionResult> Update([FromBody] UserUpdateDTO requestDTO)
    {
        if (requestDTO == null)
            return this.MissingPostModel();
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var owner = await this.GetCarmaUser();
        var clientEntry = await this.Cache.Users.UpdateSingleAsync<UserUpdateDTO, int>(requestDTO);
        if (requestDTO.IsStarred.HasValue)
            await this.Cache.Users.SetStarred(clientEntry.Entity, owner, requestDTO.IsStarred.Value);
        await this.Cache.SaveAsync();
        clientEntry.Entity.LoadParentConfig(owner);
        var responseDTO = this.Mapper.Map<UserDTO>(clientEntry.Entity);
        return this.Ok(responseDTO);
    }

    /// <summary>
    /// Delete a client
    /// </summary>
    /// <param name="accountId">User account ID.</param>
    /// <returns></returns>
    [HttpDelete("{accountId}")]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public async Task<IActionResult> Delete([FromRoute] Guid? accountId)
    {
        if (accountId == null)
            return this.MissingRouteId();

        await this.Cache.Users.DeleteAsync(accountId.Value);
        await this.Cache.SaveAsync().ConfigureAwait(false);
        return this.Success();
    }
}
