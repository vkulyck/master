using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Services.Printing;
using GmWeb.Logic.Utility.Identity.DTO;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Common.Models.Carma;
using GmWeb.Web.Api.Utility;
using GmWeb.Web.Api.Utility.Attributes;
using GmWeb.Logic.Utility.Performance.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GmWeb.Web.Api.Controllers.Common;

/// <summary>
/// The primary CRUD/view controller for Client users.
/// </summary>
public class ClientController : UserController, IDisposable
{
    private readonly UserZplPrinterService _printerService;

    public ClientController(CarmaContext context, UserManager<GmIdentity> manager, UserZplPrinterService printerService, IWebHostEnvironment env)
        : base(context, manager, env)
    {
        _printerService = printerService;
    }

    /// <summary>
    /// Get the specified client
    /// </summary>
    /// <returns>The client details DTO.</returns>
    [HttpGet]
    [ActionName("")]
    [UseSuccessModel(typeof(UserDetailsDTO))]
    [UseBadRequestModel]
    public override async Task<IActionResult> Get([FromQuery] LookupUserDTO lookup)
    {
        var current = await this.GetCarmaUser();
        var client = await this.Cache.Users.LookupUserAsync(lookup);
        if (client == null)
            return this.DataNotFound();

        var registrations = client.RegisteredActivities
            .Where(x => x.Status == Logic.Enums.UserActivityStatus.Registered)
            .ToList()
        ;
        var details = this.Mapper.Map<UserDetailsDTO>(client);
        var sourceActivities = this.Cache.ActivityCalendars.GetUserActivities(registrations).ToList();
        details.Activities = this.Mapper.Map<ActivityDTO>(sourceActivities).ToList();
        details.Notes = this.Mapper.Map(client.ParentNotes).To<NoteDTO>().ToList();
        details.IsStarred = client.ParentConfigs.SingleOrDefault(x => x.OwnerID == current.UserID)?.IsStarred ?? false;
        return this.Ok(details);
    }

    /// <summary>
    /// Get the list of clients
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(IEnumerable<UserDTO>))]
    [UseBadRequestModel]
    public virtual async Task<IActionResult> ListRegistered([FromQuery] RegisteredClientListRequest request)
    {
        if (request == null)
            return this.MissingListFilters();
        var current = await this.GetCarmaUser();
        var clients = await this.Cache.UserActivities.GetRegisteredClientsAsync(current, request.ActivityID);
        var result = clients.ToPagedResult(request).Map<UserDTO>();
        return result;
    }
    /// <summary>
    /// Get the list of clients
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(IEnumerable<UserDTO>))]
    [UseBadRequestModel]
    public virtual async Task<IActionResult> ListConfirmed([FromQuery] RegisteredClientListRequest request)
    {
        if (request == null)
            return this.MissingListFilters();
        var current = await this.GetCarmaUser();
        var clients = await this.Cache.UserActivities.GetConfirmedClientsAsync(current, request.ActivityID);
        var result = clients.ToPagedResult(request).Map<UserDTO>();
        return result;
    }

    /// <summary>
    /// Get the list of clients
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(IEnumerable<UserDTO>))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    public virtual async Task<IActionResult> ListUnregistered([FromQuery] UnregisteredClientListRequest request)
    {
        var agencyError = await this.ResolveRequestAgency(request).ConfigureAwait(false);
        if (agencyError != null)
            return agencyError;
        var current = await this.GetCarmaUser(request.AgencyID);
        var clients = await this.Cache.UserActivities.GetUnregisteredClientsAsync(current, request.ActivityID);
        return clients.ToPagedResult(request).Map<UserDTO>();
    }

    /// <summary>
    /// Get the list of clients
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [UseSuccessModel(typeof(List<string>))]
    [UseBadRequestModel]
    [UseNotFoundModel]
    [ResponseCompression]
    public virtual async Task<IActionResult> GetQrPrintCommands([FromBody] List<LookupUserDTO> request)
    {
        var users = await this.Cache.Users.LookupUsersAsync(request);
        if (users.Count < request.Count)
            return this.DataNotFound();
        else if (users.Count > request.Count)
            throw new Exception($"The request yielded more query results than expected.");
        var commands = _printerService.Generator.CreateCommands(users);
        return Ok(commands);
    }
}