using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Primitives;
using GmWeb.Logic.Utility.Extensions.Collections;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarEvent = Ical.Net.CalendarComponents.CalendarEvent;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class ActivityCalendars
    {
        public async Task<ActivityCalendar> GetAgencyCalendar(int agencyID)
        {
            string filter = $"agencyId={agencyID}".ToLowerInvariant();
            var calendar = await this.SingleOrDefaultAsync(x => x.Name.ToLower().Contains(filter));
            if (calendar == null)
                throw new ArgumentException($"No activity calendar found matching AgencyId={agencyID}");
            return calendar;
        }

        public IEnumerable<Activity> GetUserActivities(IEnumerable<UserActivity> activityUsers)
        {
            foreach (var ac in activityUsers)
            {
                var range = new DateRange(ac.ActivityStart);
                var occurrences = this.Cache.CalendarFiles.GetOccurrences<Activity>(
                    ac.ActivityCalendar, range, 
                    eventID: ac.ActivityEventID, occurrenceID: ac.ActivityID
                );
                foreach (var occurrence in occurrences)
                    yield return occurrence;
            }
        }
        public async Task<CalendarSchedule> GetScheduleAsync(int agencyID, Guid scheduleID)
        {
            var calFile = await this.Cache.ActivityCalendars.GetAgencyCalendar(agencyID);
            var schedule = this.Cache.CalendarFiles.GetSchedule(calFile, scheduleID);
            return schedule;
        }
        public async Task<Activity> GetOccurrenceAsync(int agencyID, Guid occurrenceID)
        {
            var calFile = await this.Cache.ActivityCalendars.GetAgencyCalendar(agencyID);
            return await this.GetOccurrenceAsync(calFile.CalendarID, occurrenceID);
        }
        public async Task<Activity> GetOccurrenceAsync(Guid calendarID, Guid occurrenceID)
        {
            var calFile = await this.Cache.ActivityCalendars.SingleOrDefaultAsync(x => x.CalendarID == calendarID);
            var occurrences = this.Cache.CalendarFiles.GetOccurrences<Activity>(calFile, occurrenceID: occurrenceID);
            var activity = occurrences.SingleOrDefault();
            if (activity == null)
                throw new ArgumentException($"No activity found having OccurrenceID={occurrenceID}");
            return activity;
        }
        public async Task<IEnumerable<Activity>> GetActivities(ActivityCalendar calendar, DateRange range)
            => await this.GetActivities(clientID: null, calendar, range);
        public async Task<IEnumerable<Activity>> GetActivities(int? clientID, ActivityCalendar calendar, DateRange range)
        {
            var activities = this.Cache.CalendarFiles.GetOccurrences<Activity>(calendar, range);
            if (clientID.HasValue)
            {
                var attendance = await this.Cache.UserActivities.GetAttendance(clientID.Value, calendar, range)
                    .Select(x => x.ActivityID)
                    .ToHashSetAsync()
                ;
                activities = activities.Where(x => attendance.Contains(x.ActivityID));
            }
            activities = activities.OrderBy(x => x.StartTime);
            return activities;
        }

        public async Task<CalendarSchedule> InsertScheduleAsync(Activity activity)
        {
            var schedule = Mapper.Map<Activity, CalendarSchedule>(activity);
            return await this.InsertScheduleAsync(schedule);
        }
        public async Task<CalendarSchedule> InsertScheduleAsync(CalendarSchedule schedule)
        {
            string filter = $"agencyId={schedule.AgencyID}".ToLowerInvariant();
            var calFile = this.SingleOrDefault(x => x.Name.ToLower().Contains(filter));
            if (calFile == null)
                throw new ArgumentException($"No activity calendar found matching AgencyId={schedule.AgencyID}");
            var cal = calFile.Deserialize();
            var calEvent = cal.Create<CalendarEvent>();
            schedule.CalendarID = calFile.CalendarID;
            schedule.EventID = Guid.NewGuid();
            if (schedule.RecurrenceRules.Count == 0)
                schedule.RecurrenceRules.Add(CalendarRecurrence.SingleOccurrence);
            schedule.Store(this.Cache, calEvent);
            calFile.Serialize(cal);
            return await Task.FromResult(schedule);
        }

        public async Task<CalendarSchedule> UpdateScheduleAsync(CalendarSchedule schedule)
        {
            string filter = $"agencyId={schedule.AgencyID}".ToLowerInvariant();
            var calFile = await this.SingleOrDefaultAsync(x => x.Name.ToLower().Contains(filter));
            if (calFile == null)
                throw new ArgumentException($"No activity calendar found matching AgencyId={schedule.AgencyID}");
            var cal = calFile.Deserialize();
            var calEvent = cal.Events.SingleOrDefault(x => Guid.Parse(x.Uid) == schedule.EventID);
            if (calEvent == null)
                throw new ArgumentException($"No matching event found for activity {schedule.EventID}");
            schedule.Store(this.Cache, calEvent);
            calFile.Serialize(cal);
            return schedule;
        }

        public async Task<bool> TryDeleteScheduleAsync(int agencyID, Guid eventID)
        {
            try
            {
                await this.DeleteScheduleAsync(agencyID, eventID);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        public async Task DeleteScheduleAsync(int agencyID, Guid eventID)
        {
            string filter = $"agencyId={agencyID}".ToLowerInvariant();
            var calFile = this.SingleOrDefault(x => x.Name.ToLower().Contains(filter));
            if (calFile == null)
                throw new ArgumentException($"No activity calendar found matching AgencyId={agencyID}");
            var cal = calFile.Deserialize();
            var calEvent = cal.Events.SingleOrDefault(x => Guid.Parse(x.Uid) == eventID);
            if (calEvent == null)
                throw new ArgumentException($"No matching event found for activity {eventID}");
            cal.Events.Remove(calEvent);
            calFile.Serialize(cal);
            await this.SaveAsync();
        }
    }
}
