using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Carma.User;
using Agency = GmWeb.Logic.Data.Models.Carma.Agency;
using ActivityCalendar = GmWeb.Logic.Data.Models.Carma.ActivityCalendar;

using Microsoft.AspNetCore.Identity;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Primitives;
using Gender = GmWeb.Logic.Enums.Gender;
using UserRole = GmWeb.Logic.Enums.UserRole;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using EnumExtensions = GmWeb.Logic.Utility.Extensions.Enums.EnumExtensions;

namespace GmWeb.Tests.Api.Data
{
    using static GmWeb.Tests.Api.Data.FileReader;
    public class CommonContextInitializer : ContextInitializer
    {
        protected override string ConnectionName => "Carma";
        private static readonly Random Random = new Random();
        private static readonly List<string> LanguageCodes = PrimaryLanguages.AllCodes.ToList();
        public static CommonContextInitializer Instance { get; private set; }

        public CommonContextInitializer(
            DataEntities entities,
            CarmaContext commonContext, GmIdentityContext idContext,
            UserManager<GmIdentity> manager,
            IConfiguration configuration,
            IWebHostEnvironment env
        ) : base(entities, commonContext, idContext, manager, configuration, env) { }

        private Task<User> createClient() => createUser(default(GmIdentity), UserRole.Client);
        private Task<User> createClient(GmIdentity identity) => createUser(identity, UserRole.Client);
        private Task<User> createStaffer(GmIdentity identity) => createUser(identity, UserRole.Staff);
        private async Task<User> createUser(GmIdentity identity, UserRole role)
        {
            var faker = new Bogus.Faker();
            var gender =
                Random.Next() % 20 == 0 ? Gender.NonBinary
                : Random.Next() % 5 == 0 ? Gender.Unspecified
                : faker.Person.Gender == Bogus.DataSets.Name.Gender.Male ? Gender.Male
                : faker.Person.Gender == Bogus.DataSets.Name.Gender.Female ? Gender.Female
                : Gender.Unspecified
            ;
            var client = new User
            {
                Agency = this.Entities.Agency,
                AccountID = identity?.Id,
                Email = identity?.Email ?? faker.Person.Email,
                Title = faker.Name.JobTitle(),
                FirstName = faker.Name.LastName(faker.Person.Gender),
                LastName = faker.Name.FirstName(faker.Person.Gender),
                Phone = faker.Person.Phone,
                Gender = gender,
                UserRole = UserRole.Client,
                LanguageCode = LanguageCodes[Random.Next(LanguageCodes.Count)]
            };
            return await Task.FromResult(client);
        }
        private async Task<Note> createNote(User author, User subject)
        {
            var note = new Note
            {
                NoteID = Guid.NewGuid(),
                NoteAuthor = author,
                NoteSubject = subject,
                ModifiedBy = author,
                Created = DateTimeOffset.Now,
                Modified = null,
                Title = $"Title by {author.UserID}: {author.Email}",
                Message = $"Message by {author.UserID}: {author.Email}",
                Status = NoteStatus.Flagged
            };
            return await Task.FromResult(note);
        }
        protected override async Task Populate()
        {
            var cache = new CarmaCache(this.ComCtx);
            this.Entities.Agency = new Agency
            {
                Name = "Test Agency"
            };
            this.ComCtx.Agencies.Add(this.Entities.Agency);

            this.Entities.AdminStaffer = await createStaffer(identity: this.Entities.AdminIdentity);
            this.ComCtx.Users.Add(this.Entities.AdminStaffer);

            this.Entities.MemberClient = await createClient(identity: this.Entities.MemberIdentity);
            this.ComCtx.Users.Add(this.Entities.MemberClient);

            for (int i = 0; i < 100; i++)
            {
                var client = await createClient();
                this.ComCtx.Users.Add(client);
            }
            await this.ComCtx.SaveAsync();
            for (int i = 0; i < 1; i++)
            {
                var author = this.Entities.AdminStaffer;
                var subject = this.ComCtx.Users.Skip(i).FirstOrDefault();
                var note = await this.createNote(author, subject);
                this.ComCtx.Notes.Add(note);
                this.Entities.AdminNote = note;
            }
            await this.ComCtx.SaveAsync();
            cache.Threads.AddRange(await cache.Threads.ConvertFromNotes());
            await this.ComCtx.SaveAsync();
            var profilePictureRoot = Path.Combine(this.Env.WebRootPath, "Images", "UserProfile");
            foreach (var user in this.ComCtx.Users)
                File.Copy($"{profilePictureRoot}/default-user-profile-image.jpg", $"{profilePictureRoot}/{user.LookupID}.jpg");

            this.Entities.ActivityCalendar = new ActivityCalendar
            {
                Name = this.Entities.Agency.ActivityCalendarFilename,
                FileStream = ReadCalendar(this.Entities.Agency.ActivityCalendarFilename)
            };
            this.ComCtx.ActivityCalendars.Add(this.Entities.ActivityCalendar);

            var occurrences = cache.CalendarFiles.GetOccurrences<Activity>(this.Entities.ActivityCalendar,
                new DateRange(Logic.Enums.TimePeriod.Weekly, DateTime.Today.AddDays(7)),
                eventID: null,
                occurrenceID: null
            ).ToList();
            var regActivity = new UserActivity
            {
                Registrant = this.Entities.MemberClient,
                Registrar = this.Entities.AdminStaffer,
                ActivityCalendar = this.Entities.ActivityCalendar,
                ActivityEventID = occurrences[0].EventID,
                ActivityID = occurrences[0].OccurrenceID,
                ActivityStart = occurrences[0].StartTime,
                DateRegistered = DateTime.Now,
                DateConfirmed = default(DateTime?),
                Status = Logic.Enums.UserActivityStatus.Registered
            };
            this.ComCtx.UserActivities.Add(regActivity);
            var confActivity = new UserActivity
            {
                Registrant = this.Entities.MemberClient,
                Registrar = this.Entities.AdminStaffer,
                ActivityCalendar = this.Entities.ActivityCalendar,
                ActivityEventID = occurrences[1].EventID,
                ActivityID = occurrences[1].OccurrenceID,
                ActivityStart = occurrences[1].StartTime,
                DateRegistered = DateTime.Now,
                DateConfirmed = DateTime.Now,
                Status = Logic.Enums.UserActivityStatus.AttendanceConfirmed
            };
            this.ComCtx.UserActivities.Add(confActivity);

            this.Entities.Activities = (await cache.ActivityCalendars.GetActivities(
                this.Entities.ActivityCalendar,
                new DateRange(Logic.Enums.TimePeriod.Daily)
            )).ToList();

            await Task.CompletedTask;
        }
        protected override async Task Validate()
        {
            if (this.ComCtx == null)
                throw new NullReferenceException("Cannot get instance of dbContext");

            if (this.ConnectionString.ToLower().Contains("GmIdentity") | this.ConnectionString.ToLower().Contains("GmCommon"))
                throw new Exception("LIVE SETTINGS IN TESTS!");

            await this.ComCtx.Database.EnsureDeletedAsync();
            await this.ComCtx.Database.EnsureCreatedAsync();
        }
        protected override async Task<int> SaveChangesAsync()
        {
            var count = 0;
            count += await this.ComCtx.SaveChangesAsync();
            count += await this.IdCtx.SaveChangesAsync();
            return count;
        }
        protected override int SaveChanges()
        {
            var count = 0;
            count += this.ComCtx.SaveChanges();
            count += this.IdCtx.SaveChanges();
            return count;
        }
    }
}
