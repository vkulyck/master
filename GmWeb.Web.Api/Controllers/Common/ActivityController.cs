using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Extensions.Chronometry;
using GmWeb.Logic.Utility.Primitives;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Utility;
using GmWeb.Web.Api.Utility.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TimePeriod = GmWeb.Logic.Enums.TimePeriod;

namespace GmWeb.Web.Api.Controllers.Common;

/// <summary>
/// The primary CRUD/view controller for Activity instances.
/// </summary>
public class ActivityController : CarmaController, IDisposable
{
    private readonly ActivitySettings _settings;
    public ActivityController(
        CarmaContext context, 
        UserManager<GmIdentity> manager, 
        IWebHostEnvironment env,
        IOptions<ActivitySettings> settings
    ) : base(context, manager, env) 
    {
        _settings = settings.Value;
    }

    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<ActivityDetailsDTO>), description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, type: typeof(object), description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, type: typeof(object), description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, type: typeof(object), description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, type: typeof(object), description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> ListDaily([FromQuery] ActivityListRequest request)
        => await this.ListActivities(request with { Period = TimePeriod.Daily }).ConfigureAwait(false);
    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<ActivityDetailsDTO>), description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, type: typeof(object), description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, type: typeof(object), description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, type: typeof(object), description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, type: typeof(object), description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> ListWeekly([FromQuery] ActivityListRequest request)
        => await this.ListActivities(request with { Period = TimePeriod.Weekly }).ConfigureAwait(false);
    [HttpGet]
    [ValidateModelState]
    [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<ActivityDetailsDTO>), description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, type: typeof(object), description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, type: typeof(object), description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, type: typeof(object), description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, type: typeof(object), description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> ListMonthly([FromQuery] ActivityListRequest request)
        => await this.ListActivities(request with { Period = TimePeriod.Monthly }).ConfigureAwait(false);

    protected virtual async Task<IActionResult> ListActivities(ActivityListRequest request)
    {
        var agencyError = await this.ResolveRequestAgency(request).ConfigureAwait(false);
        if (agencyError != null)
            return agencyError;
        var periodCount = _settings.GetMaximumPeriodCount(request.Period);
        var range = new DateRange
        (
            period: request.Period,
            referenceDate: request.PageDate,
            periodScale: periodCount,
            periodShift: request.PageIndex
        );
        var calendar = await this.Cache.ActivityCalendars.GetAgencyCalendar(request.AgencyID.Value);
        var sourceActivities = await this.Cache.ActivityCalendars.GetActivities(request.ClientID, calendar, range);

        var extRequest = request.Extend() as ExtendedTimePeriodListRequest<Activity>;
        var maxItemCount = this._settings.GetTargetActivityCount(request.Period);
        List<ExtendedPagedList<ActivityDetailsDTO, DateTime>> pages = new();
        for (int i = 0; i < periodCount; i++)
        {
            var date = range.Start.NextPeriodStart(request.Period, i);
            var dateRequest = extRequest with { GroupKey = date };
            var paged = sourceActivities.Page(dateRequest);
            var mapped = paged.Map<ActivityDetailsDTO>();
            pages.Add(mapped);
            if (pages.Sum(x => x.Items.Count) >= maxItemCount)
                break;
        }
        var result = pages.ToResult();
        return result;
    }

    [HttpPost]
    [ValidateModelState]
    [ProducesResponseType(typeof(UserActivityDTO), 200)]
    [SwaggerResponse(statusCode: 200, description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> ConfirmClient([FromBody] ConfirmClientRequest request)
    {
        if (request == null)
            return this.MissingPostModel();
        var staff = await this.GetCarmaUser();
        var status = await this.Cache.UserActivities.ConfirmClient(staff.UserID, request.LookupID, request.ActivityID);
        var dto = this.Mapper.Map<UserActivityDTO>(status);
        return this.Success(dto);
    }

    [HttpPost]
    [ValidateModelState]
    [ProducesResponseType(typeof(UserActivityDTO), 200)]
    [SwaggerResponse(statusCode: 200, description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> RegisterClient([FromBody] RegisterClientRequest request)
    {
        if (request == null)
            return this.MissingPostModel();
        var staff = await this.GetCarmaUser();
        var status = await this.Cache.UserActivities.RegisterClient(staff.UserID, request.LookupID, request.ActivityID);
        var dto = this.Mapper.Map<UserActivityDTO>(status);
        return this.Success(dto);
    }

    [HttpPost]
    [ValidateModelState]
    [ProducesResponseType(typeof(UserActivityDTO), 200)]
    [SwaggerResponse(statusCode: 200, description: "Successful operation.")]
    [SwaggerResponse(statusCode: 400, description: "Invalid data supplied for the request.")]
    [SwaggerResponse(statusCode: 401, description: "The requested action has not been authorized.")]
    [SwaggerResponse(statusCode: 403, description: "The requested action is not permitted.")]
    [SwaggerResponse(statusCode: 404, description: "The specified resource was not found.")]
    public virtual async Task<IActionResult> UnregisterClient([FromBody] UnregisterClientRequest request)
    {
        if (request == null)
            return this.MissingPostModel();
        var staff = await this.GetCarmaUser();
        var status = await this.Cache.UserActivities.UnregisterClient(staff.UserID, request.LookupID, request.ActivityID);
        var dto = this.Mapper.Map<UserActivityDTO>(status);
        return this.Success(dto);
    }

    /// <summary>
    /// Insert an activity
    /// </summary>
    /// <param name="requestDTO"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CalendarDetailsDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Insert([FromBody] ScheduleInsertDTO requestDTO)
    {
        if (requestDTO == null)
            return this.MissingPostModel();
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        var schedule = this.Mapper.Map<CalendarSchedule>(requestDTO);
        if (requestDTO.OrganizerID.HasValue)
            schedule.Organizer = await this.Cache.Users.SingleOrDefaultAsync(x => x.UserID == requestDTO.OrganizerID);
        schedule.Organizer ??= await this.GetCarmaUser();
        await this.Cache.ActivityCalendars.InsertScheduleAsync(schedule);
        await this.Cache.SaveAsync();
        var responseDTO = this.Mapper.Map<CalendarDetailsDTO>(schedule);
        return this.Success(responseDTO);
    }

    /// <summary>
    /// Update an activity
    /// </summary>
    /// <param name="requestDTO"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(CalendarDetailsDTO), 200)]
    [UseBadRequestModel]
    public async Task<IActionResult> Update([FromBody] ScheduleUpdateDTO requestDTO)
    {
        if (requestDTO == null)
            return this.MissingPostModel();
        if (!this.ModelState.IsValid)
            return this.ModelStateErrors();

        requestDTO.AgencyID ??= await this.GetAgencyID();
        var schedule = await this.Cache.ActivityCalendars.GetScheduleAsync(requestDTO.AgencyID.Value, requestDTO.EventID);
        if (schedule == null)
            return this.DataNotFound();
        this.Mapper.Map(requestDTO, schedule);
        await this.Cache.ActivityCalendars.UpdateScheduleAsync(schedule);
        await this.Cache.SaveAsync();
        var resultDTO = this.Mapper.Map<CalendarDetailsDTO>(schedule);
        return this.Success(resultDTO);
    }

    /// <summary>
    /// Delete an activity
    /// </summary>
    /// <param name="agencyID">The agency associated with the provided activity. If ommitted, the current login's AgencyID will be used.</param>
    /// <param name="eventID">The ID of the activity event to be deleted.</param>
    /// <returns></returns>
    [HttpDelete("{eventID}")]
    [UseStatusCode(HttpStatusCode.OK)]
    [UseBadRequestModel]
    public async Task<IActionResult> Delete([FromRoute] Guid eventID, [FromQuery] int? agencyID)
    {
        if (agencyID == null)
        {
            var user = await this.GetCarmaUser();
            agencyID = user.AgencyID;
        }
        // TODO: Consider moving SaveAsync out of CRUD methods and into endpoint handlers to group operations into transactions.
        bool acDeleted = await this.Cache.ActivityCalendars.TryDeleteScheduleAsync(agencyID.Value, eventID).ConfigureAwait(false);
        bool uaDeleted = await this.Cache.UserActivities.TryDeleteAsync(eventID);
        string message;
        if (acDeleted && uaDeleted)
            message = "Activity and registrations were deleted successfully.";
        else if (acDeleted)
            message = "Activity deleted successfully from the calendar.";
        else if (uaDeleted)
            message = "Activity registrations were deleted succesfully, but no matching activity was found in the calendar.";
        else
            throw new Exception("The provided activity could not be found in the calendar or in registration references.");
        return this.Success(new
        {
            success = true,
            message
        });
    }
}
