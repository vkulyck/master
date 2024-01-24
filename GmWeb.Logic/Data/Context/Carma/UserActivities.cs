using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Primitives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class UserActivities
    {
        protected async Task<UserActivity> FindUserActivity(Guid lookupID, Guid activityID)
        {
            var user = await this.Cache.Users.LookupClientAsync(lookupID);
            var clientActivity = await this
                .Where(x => x.RegistrantID == user.UserID)
                .Where(x => x.ActivityID == activityID)
                .SingleOrDefaultAsync()
            ;
            return clientActivity;
        }
        protected async Task<UserActivity> InsertUserActivity(int registrarID, Guid lookupID, Guid activityID)
        {
            var guid = new ActivityGuid(activityID);
            var client = await this.Cache.Users.LookupClientAsync(lookupID);
            var calendar = await this.Cache.ActivityCalendars.GetAgencyCalendar(client.AgencyID);
            var activity = await this.Cache.ActivityCalendars.GetOccurrenceAsync(calendar.CalendarID, activityID);
            var clientActivity = this.Insert(new UserActivity
            {
                ActivityCalendarID = calendar.CalendarID,
                ActivityEventID = activity.EventID,
                ActivityID = activityID,
                RegistrantID = client.UserID,
                RegistrarID = registrarID,
                ActivityStart = guid.ActivityStartTime,
                DateRegistered = default,
                DateConfirmed = default,
                Status = UserActivityStatus.Unregistered
            });
            return clientActivity;
        }
        public async Task<UserActivity> UnregisterClient(int registrarID, Guid lookupID, Guid activityID)
        {
            var clientActivity = await this.FindUserActivity(lookupID, activityID);
            if (clientActivity == null)
                clientActivity = await this.InsertUserActivity(registrarID, lookupID, activityID);
            clientActivity.DateRegistered = default;
            clientActivity.Status = UserActivityStatus.Unregistered;
            await this.SaveAsync();
            return clientActivity;
        }
        public async Task<UserActivity> RegisterClient(int registrarID, Guid lookupID, Guid activityID)
        {
            var clientActivity = await this.FindUserActivity(lookupID, activityID);
            if (clientActivity == null)
                clientActivity = await this.InsertUserActivity(registrarID, lookupID, activityID);
            clientActivity.DateRegistered = DateTime.Now;
            clientActivity.Status = UserActivityStatus.Registered;
            await this.SaveAsync();
            return clientActivity;
        }
        public async Task<UserActivity> ConfirmClient(int registrarID, Guid lookupID, Guid activityID)
        {
            var clientActivity = await this.FindUserActivity(lookupID, activityID);
            if (clientActivity == null)
                clientActivity = await this.InsertUserActivity(registrarID, lookupID, activityID);
            if (clientActivity.Status == UserActivityStatus.Unregistered)
                return clientActivity;
            if(clientActivity.Status == UserActivityStatus.Registered)
                clientActivity.DateConfirmed = DateTime.Now;
            clientActivity.Status = UserActivityStatus.AttendanceConfirmed;
            await this.SaveAsync();
            return clientActivity;
        }

        public async Task<List<User>> GetUnregisteredClientsAsync(User current, Guid activityID)
        {
            var calendar = await this.Cache.ActivityCalendars.GetAgencyCalendar(current.AgencyID);
            var unregistered = new List<User>();
            var activity = await this.Cache.ActivityCalendars.GetOccurrenceAsync(calendar.CalendarID, activityID);
            var explicitClients = await this.GetActivityClientsAsync(current, activityID, UserActivityStatus.Registered);
            var existingClients = await this
                .Where(x => x.ActivityID == activityID)
                .Select(x => x.RegistrantID)
                .ToHashSetAsync()
            ;
            var implicitClients = await this.Cache.Users.GetAgencyClients(current)
                .Where(x => !existingClients.Contains(x.UserID))
                .Distinct()
                .ToListAsync()
            ;
            var allClients = explicitClients.Union(implicitClients).ToList();
            allClients.ForEach(x => x.LoadParentConfig(current));
            return allClients;
        }

        public async Task<List<User>> GetRegisteredClientsAsync(User current, Guid activityID)
            => await this.GetActivityClientsAsync(current, activityID, UserActivityStatus.Registered);
        public async Task<List<User>> GetConfirmedClientsAsync(User current, Guid activityID)
            => await this.GetActivityClientsAsync(current, activityID, UserActivityStatus.AttendanceConfirmed);
        public async Task<List<User>> GetActivityClientsAsync(User current, Guid activityID, UserActivityStatus status)
        {
            var clientActivities = await this
                .Where(x => x.ActivityID == activityID)
                .Where(x => x.Status == status)
                .Select(x => x.RegistrantID)
                .ToHashSetAsync()
            ;
            var clients = await this.Cache.Users.GetAgencyClients(current)
                .Where(x => clientActivities.Contains(x.UserID))
                .ToListAsync()
            ;
            clients.ForEach(x => x.LoadParentConfig(current));
            return clients;
        }
        public async Task<List<Activity>> GetRegisteredClientActivitiesAsync(User client)
            => await this.GetClientActivitiesAsync(client, UserActivityStatus.Registered);
        public async Task<List<Activity>> GetRegisteredClientActivitiesAsync(int clientID)
            => await this.GetClientActivitiesAsync(clientID, UserActivityStatus.Registered);
        public async Task<List<Activity>> GetConfirmedClientActivitiesAsync(User client)
            => await this.GetClientActivitiesAsync(client, UserActivityStatus.AttendanceConfirmed);
        public async Task<List<Activity>> GetConfirmedClientActivitiesAsync(int clientID)
            => await this.GetClientActivitiesAsync(clientID, UserActivityStatus.AttendanceConfirmed);
        protected async Task<List<Activity>> GetClientActivitiesAsync(User client, UserActivityStatus status)
            => await this.GetClientActivitiesAsync(client.UserID, status);
        protected async Task<List<Activity>> GetClientActivitiesAsync(int clientID, UserActivityStatus status)
            => await this.GetClientActivitiesAsync(clientID: clientID, activityID: default(Guid?), status: status);

        private struct ActivityRegistration
        {
            private Guid ActivityID { get; set; }
            private DateTime ActivityStart { get; set; }
            private UserActivityStatus ClientStatus { get; set; }
        }

        protected Expression<Func<UserActivity, bool>> IsContainedBy(DateRange range)
        {
            return (UserActivity ua) =>
                range.Start <= ua.ActivityStart && ua.ActivityStart < range.End
            ;
        }

        // TODO: Merge code with GetClientActivitiesAsync
        public IQueryable<UserActivity> GetAttendance(ActivityCalendar calendar, DateRange range)
            => this.GetAttendance(clientID: null, calendar, range);
        // TODO: Merge code with GetClientActivitiesAsync
        public IQueryable<UserActivity> GetAttendance(int? clientID, ActivityCalendar calendar, DateRange range)
        {
            var attendance = this.Cache.UserActivities
                .Where(x => x.ActivityCalendarID == calendar.CalendarID)
                .Where(x => (x.Status & UserActivityStatus.IsAttending) != UserActivityStatus.Unregistered)
                .Where(IsContainedBy(range))
            ;
            if (clientID.HasValue)
                attendance = attendance.Where(x => x.RegistrantID == clientID);
            return attendance;
        }
        // TODO: Merge code with GetAttendance
        protected async Task<List<Activity>> GetClientActivitiesAsync(int? clientID, Guid? activityID, UserActivityStatus status)
        {
            var activities = new List<Activity>();
            var registrations = this as IQueryable<UserActivity>;
            if (clientID.HasValue)
                registrations = registrations.Where(x => x.RegistrantID == clientID.Value);
            if (activityID != null)
                registrations = registrations.Where(x => x.ActivityID == activityID);
            var calGroups = await registrations.GroupBy(x => x.ActivityCalendarID).ToListAsync();
            foreach (var cg in calGroups)
            {
                var calFile = await this.Cache.ActivityCalendars.SingleOrDefaultAsync(x => x.CalendarID == cg.Key);
                if (calFile == null)
                    throw new NullReferenceException($"No calendar file found matching ActivityCalendarID={cg.Key} for ClientID={clientID}");
                foreach (var reg in cg)
                {
                    if (reg.Status != status)
                        continue;
                    var range = new DateRange(reg.ActivityStart);
                    var calEvents = this.Cache.CalendarFiles.GetOccurrences<Activity>(calFile, range, occurrenceID: activityID);
                    var regActivity = calEvents.SingleOrDefault(x => x.ActivityID == reg.ActivityID);
                    if (regActivity == null)
                        throw new NullReferenceException($"No occurrences found for ClientID={clientID}, ActivityCalendarID={cg.Key} from {range.Start:yyyy-MM-dd}");
                    activities.Add(regActivity);
                }
            }
            return activities;
        }

        public async Task<bool> TryDeleteAsync(Guid eventID)
        {
            int count = await this.DeleteAsync(eventID);
            return count > 0;
        }
        public async Task<int> DeleteAsync(Guid eventID)
        {
            int count = await this.CountAsync(x => x.ActivityEventID == eventID);
            this.EntitySet.RemoveAll(x => x.ActivityEventID == eventID);
            await this.SaveAsync();
            return count;
        }
    }
}
