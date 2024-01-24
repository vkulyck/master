using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Logic.Utility.Primitives;
using GmWeb.Web.Api.Models.Common;
using GmWeb.Web.Api.Utility.Attributes;
using GmWeb.Web.Common.Models.Carma;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Api.Controllers.Common;

/// <summary>
/// The primary CRUD/view controller for Client users.
/// </summary>
public class HomeController : CarmaController, IDisposable
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(CarmaContext context, UserManager<GmIdentity> manager, IWebHostEnvironment env, ILoggerFactory loggerFactory) 
        : base(context, manager, env) 
    {
        _logger = loggerFactory.CreateLogger<HomeController>();
    }

    /// <summary>
    /// Get the specified user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [UseSuccessModel(typeof(HomeSummaryDTO))]
    [UseBadRequestModel]
    public async Task<IActionResult> Summary([FromQuery] DateTime? date, int? AgencyID)
    {        
        try
        {
            var refDate = date?.Date ?? DateTime.Today;
            var current = await this.GetCarmaUser(AgencyID);
            var calendar = await this.Cache.ActivityCalendars.GetAgencyCalendar(current.AgencyID);
            var range = new DateRange(TimePeriod.Daily, referenceDate: refDate);

            var dailyActivities = await this.Cache.ActivityCalendars.GetActivities(calendar, range);
            var attendance = this.Cache.UserActivities.GetAttendance(calendar, range);
            var todaysActiveClients = attendance
                .Select(x => x.Registrant)
                .Where(x => x.UserRole == UserRole.Client)
            ;
            var starredClients = this.Cache.Users.GetAgencyClients(current, ViewerFilter.Where(UserConfigStatus.Starred));
            var summary = new HomeSummaryDTO
            {
                TodayActivityCount = dailyActivities.Count(),
                ClientsWithActivitiesTodayCount = await todaysActiveClients.CountAsync(),
                StarredClientCount = await starredClients.CountAsync()
            };
            return this.Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching home summary data.");
            return this.BadRequest("Error fetching data.");
        }
    }

    protected async IAsyncEnumerable<User> GetRelevantClients(ActivityCalendar calendar, DateRange range)
    {
        var current = await this.GetCarmaUser();
        var attendingClients = await this.Cache.UserActivities.GetAttendance(calendar, range)
            .Select(x => x.Registrant)
            .ToListAsync()
        ;
        var starredClients = await this.Cache.Users
            .GetAgencyClients(current, ViewerFilter.Where(UserConfigStatus.Starred))
            .ToListAsync()
        ;
        var combined = starredClients.Concat(attendingClients).ToList();
        var seen = new HashSet<int>();
        foreach (var client in combined)
        {
            if (seen.Contains(client.UserID))
                continue;
            seen.Add(client.UserID);
            yield return client;
        }
    }

    /// <summary>
    /// Get the specified user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ActionName("")]
    [UseSuccessModel(typeof(HomeDTO))]
    [UseBadRequestModel]
    public async Task<IActionResult> Get([FromQuery] DateTime? date)
    {
        date = date?.Date ?? DateTime.Today;
        var current = await this.GetCarmaUser();
        var calendar = await this.Cache.ActivityCalendars.GetAgencyCalendar(current.AgencyID);
        var range = new DateRange(TimePeriod.Daily, referenceDate: date);

        var activities = (await this.Cache.ActivityCalendars.GetActivities(calendar, range))
            .ToList()
            .Page()
            .Map<ActivityDetailsDTO>()
        ;
    
        var clients = (await GetRelevantClients(calendar, range).ToListAsync())
            .Page()
            .Map<UserDTO>()
        ;
        var notes = await this.Cache.Threads
            .Where(x => x.AuthorID == current.UserID)
            .PageAsync()
            .MapAsync<Thread, ThreadDTO>()
        ;

        try
        {
            var dto = new HomeDTO
            {
                Activities = activities,
                AssignedClients = clients,
                Notes = notes
            };
            return this.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching home data.");
            return this.BadRequest("Error fetching data.");
        }
    }
}
