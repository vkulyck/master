using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;

using UserRole = GmWeb.Logic.Enums.UserRole;
using User = GmWeb.Logic.Data.Models.Carma.User;
using UserDTO = GmWeb.Web.Common.Models.Carma.UserDTO;
using UserActivityDTO = GmWeb.Web.Api.Models.Common.UserActivityDTO;
using ActivityInsert = GmWeb.Web.Api.Models.Common.ScheduleInsertDTO;
using ActivityUpdate = GmWeb.Web.Api.Models.Common.ScheduleUpdateDTO;
using ActivityDetails = GmWeb.Web.Api.Models.Common.ActivityDetailsDTO;
using ClientDetailsDTO = GmWeb.Web.Api.Models.Common.UserDetailsDTO;
using Recurrence = GmWeb.Logic.Data.Models.Carma.CalendarRecurrence;
using Startup = GmWeb.Web.Api.Startup;
using GmWeb.Tests.Api.Mocking;
using GmWeb.Tests.Api.Extensions;
using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Logic.Utility.Performance.Paging;
using GmWeb.Web.Api.Utility;
using UserActivityStatus = GmWeb.Logic.Enums.UserActivityStatus;
using Frequency = Ical.Net.FrequencyType;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class ActivityControllerTests : ControllerTestBase<ActivityControllerTests>
    {
        private readonly ActivitySettings _settings;
        public ActivityControllerTests(TestApplicationFactory factory) : base(factory)
        {
            var options = this.Scope.ServiceProvider.GetService<IOptions<ActivitySettings>>();
            _settings = options.Value;
        }

        [Fact]
        public async Task ValidateActivityCrudWithExplicitOrganizer()
        {
            await ValidateActivityCrud(this.Entities.AdminStaffer.UserID);
        }

        [Fact]
        public async Task ValidateActivityCrudWithImplicitOrganizer()
        {
            await ValidateActivityCrud(default);
        }

        [Fact]
        public async Task ValidateSingleActivityRecurrence()
        {
            var recurrence = new Recurrence
            {
                Frequency = Frequency.None,
                Interval = 0,
                Count = 1
            };
            var (request, details, results) = await this.ValidateActivityRecurrence(recurrence);
            Assert.Equal(recurrence.Count, results.Count);
        }

        [Fact]
        public async Task ValidateDailyActivityRecurrence()
        {
            var recurrence = new Recurrence
            {
                Frequency = Frequency.Daily,
                Interval = 1,
                Count = 2
            };
            var (request, details, results) = await this.ValidateActivityRecurrence(recurrence);
            Assert.Equal(recurrence.Count, results.Count);
            Assert.Equal(request.StartTime, results[0].StartTime);
            Assert.Equal(request.StartTime.AddDays(1), results[1].StartTime);
        }

        [Fact]
        public async Task ValidateBiweeklyActivityRecurrence()
        {
            var recurrence = new Recurrence
            {
                Frequency = Frequency.Weekly,
                Interval = 2,
                Count = 2
            };
            var (request, details, results) = await this.ValidateActivityRecurrence(recurrence);
            Assert.Equal(recurrence.Count, results.Count);
            Assert.Equal(request.StartTime, results[0].StartTime);
            Assert.Equal(request.StartTime.AddDays(14), results[1].StartTime);
        }

        protected async Task<(ActivityInsert Request, ActivityDetails Details, List<ActivityDetails> Occurrences)> ValidateActivityRecurrence(Recurrence recurrence)
            => await this.ValidateActivityRecurrence(recurrence, organizerID: default);
        protected async Task<(ActivityInsert Request, ActivityDetails Details, List<ActivityDetails> Occurrences)> ValidateActivityRecurrence(Recurrence recurrence, int? organizerID)
        {
            var ent = this.Entities;
            var start = DateTime.Today.AddDays(1).AddHours(8).AddMinutes(27);
            var insertDTO = new ActivityInsert
            {
                OrganizerID = organizerID,
                AgencyID = ent.AdminStaffer.AgencyID,
                Name = "Test Insertion Activity",
                StartTime = start,
                EndTime = start.AddHours(3),
                Location = "123 Test Lane, San Francisco, CA, 94103",
                Capacity = 35,
                Description = "A simple, isolated activity occurring once in the middle of the day this week.",
                EventType = "1-2+ Housing",
                Contribution = 1893.23M,
                RecurrenceRules = new() { recurrence }
            };
            var activityDetails = await this.RequestDataAsync<ActivityDetails>(
                Controller: "Activity", Action: "Insert", Method: HttpMethod.Post,
                RequestData: insertDTO,
                ExpectedStatus: HttpStatusCode.OK
            );

            Assert.Equal(insertDTO.Name, activityDetails.Name);
            Assert.Equal(insertDTO.EventType, activityDetails.EventType);
            Assert.Equal(insertDTO.Capacity, activityDetails.Capacity);
            Assert.Equal(insertDTO.Contribution, activityDetails.Contribution);

            var pagedInsertLists = await this.RequestDataAsync<List<ExtendedPagedList<ActivityDetails, DateTime>>>(
                Controller: "Activity", Action: "List-Monthly", Method: HttpMethod.Get,
                RequestData: new { ent.AdminStaffer.AgencyID, PageIndex = 0 },
                ExpectedStatus: HttpStatusCode.OK
            );

            Assert.NotEmpty(pagedInsertLists);
            foreach(var pagedInsertList in pagedInsertLists)
                Assert.NotEmpty(pagedInsertList.Items);
            var inserts = pagedInsertLists
                .SelectMany(x => x.Items)
                .Where(x => x.EventID == activityDetails.EventID)
                .ToList()
            ;
            var inserted = inserts.First();
            Assert.Equal(activityDetails.CalendarID, inserted.CalendarID);
            Assert.Equal(activityDetails.EventID, inserted.EventID);

            Assert.Equal(insertDTO.Name, inserted.Name);
            Assert.Equal(insertDTO.EventType, inserted.EventType);
            Assert.Equal(insertDTO.Capacity, inserted.Capacity);
            Assert.Equal(insertDTO.Contribution, inserted.Contribution);
            Assert.Equal(start.TimeOfDay, inserted.StartTime.TimeOfDay);
            Assert.Equal(start.Day, inserted.StartTime.Day);
            Assert.Equal(start.Month, inserted.StartTime.Month);

            return (insertDTO, activityDetails, inserts);
        }

        protected async Task ValidateActivityCrud(int? organizerID)
        {
            var ent = this.Entities;
            var recurrence = new Recurrence
            {
                Frequency = Ical.Net.FrequencyType.Daily,
                Until = DateTime.Today.AddYears(10)
            };
            var (request, details, results) = await this.ValidateActivityRecurrence(recurrence);

            var updateDTO = new ActivityUpdate
            {
                EventID = details.EventID,
                Capacity = 10000,
                Description = "changed description",
                EventType = "changed event type",
                Contribution = 222.33M
            };

            await this.RequestDataAsync<ActivityDetails>(
                Controller: "Activity", Action: "Update", Method: HttpMethod.Put,
                RequestData: updateDTO,
                ExpectedStatus: HttpStatusCode.OK
            );

            var pagedUpdatesList = await this.RequestDataAsync<List<ExtendedPagedList<ActivityDetails, DateTime>>>(
                Controller: "Activity", Action: "List-Monthly", Method: HttpMethod.Get,
                RequestData: new { ent.AdminStaffer.AgencyID, PageIndex = 1 },
                ExpectedStatus: HttpStatusCode.OK
            );

            var pagedUpdates = pagedUpdatesList.First();
            var updates = pagedUpdatesList.Last().Items.Where(x => x.EventID == details.EventID).ToList();

            Assert.NotEmpty(pagedUpdates.Items);

            var updated = updates.Last();
            Assert.Equal(details.CalendarID, updated.CalendarID);
            Assert.Equal(details.EventID, updated.EventID);

            Assert.Equal(updateDTO.EventType, updated.EventType);
            Assert.Equal(updateDTO.Capacity, updated.Capacity);
            Assert.Equal(updateDTO.Contribution, updated.Contribution);
            Assert.Equal(updateDTO.Description, updated.Description);
            Assert.Equal(details.StartTime.TimeOfDay, updated.StartTime.TimeOfDay);
            var listedMonths = _settings.MaximumListPeriodCount[Logic.Enums.TimePeriod.Monthly];
            Assert.Equal(details.StartTime.AddMonths(listedMonths).Month, updated.StartTime.Month);
        }
        [Fact]
        public async Task ValidateConfirmActivityClient()
        {
            var ent = this.Entities;
            var activity = ent.Activities.First();
            var users = this.ComCtx.Users.ToList();

            var regResponse = await this.Client.ProcessPostAsync("activity/register-client", new
            {
                ent.MemberClient.LookupID,
                ActivityID = activity.OccurrenceID,
                ActivityStart = activity.StartTime
            });
            Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);
            var regData = await regResponse.ParseBodyAsync<UserActivityDTO>();
            Assert.Equal(ent.MemberClient.LookupID, regData.Registrant.LookupID);
            Assert.Equal(UserActivityStatus.Registered, regData.Status);

            var regUsers = await this.RequestDataAsync<PagedList<UserDTO>>("Client", "List-Registered", HttpMethod.Get, RequestData: new
            {
                ActivityID = activity.OccurrenceID
            }, ExpectedStatus: HttpStatusCode.OK);

            Assert.Collection(regUsers.Items, user =>
            {
                Assert.Equal(user.UserID, ent.MemberClient.UserID);
                Assert.Equal(user.Email, ent.MemberEmail);
                Assert.Equal(user.AccountID, ent.MemberIdentity.Id);
            });

            var confResponse = await this.Client.ProcessPostAsync("activity/confirm-client", new
            {
                ent.MemberClient.LookupID,
                ActivityID = activity.OccurrenceID
            });
            Assert.Equal(HttpStatusCode.OK, confResponse.StatusCode);
            var confData = await confResponse.ParseBodyAsync<UserActivityDTO>();
            Assert.Equal(ent.MemberClient.LookupID, confData.Registrant.LookupID);
            Assert.Equal(UserActivityStatus.AttendanceConfirmed, confData.Status);

            var listConfDataBody = await this.RequestDataAsync<PagedList<UserDTO>>(
                Controller: "Client", Action: "List-Confirmed", Method: HttpMethod.Get,
                RequestData: new
                {
                    ActivityID = activity.OccurrenceID
                },
                ExpectedStatus: HttpStatusCode.OK
            );
            var listConfData = listConfDataBody.Items;
            Assert.Collection(listConfData, user =>
            {
                Assert.Equal(user.UserID, ent.MemberClient.UserID);
                Assert.Equal(user.Email, ent.MemberEmail);
                Assert.Equal(user.AccountID, ent.MemberIdentity.Id);
            });

            var unregResponse = await this.Client.ProcessPostAsync("activity/unregister-client", new
            {
                ent.MemberClient.LookupID,
                ActivityID = activity.OccurrenceID
            });
            Assert.Equal(HttpStatusCode.OK, unregResponse.StatusCode);

            var unregData = new
            {
                ent.MemberClient.AgencyID,
                ActivityID = activity.OccurrenceID,
                ActivityStart = activity.StartTime,
                PageSize = 11
            };
            var listUnregDataBody = await this.RequestDataAsync<PagedList<UserDTO>>(
                Controller: "Client", Action: "List-Unregistered", Method: HttpMethod.Get,
                RequestData: unregData, ExpectedStatus: HttpStatusCode.OK
            );
            var listUnregData = listUnregDataBody.Items;
            Assert.Equal(unregData.PageSize, listUnregData.Count);
            for (int i = 0; i < listUnregData.Count; i++)
            {
                var u = listUnregData[i];
                Assert.Equal(UserRole.Client, u.UserRole);
            }
        }
    }
}
