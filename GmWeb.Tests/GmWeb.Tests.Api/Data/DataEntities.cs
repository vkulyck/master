using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Bogus;

using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Carma.User;
using Note = GmWeb.Logic.Data.Models.Carma.Note;
using Activity = GmWeb.Logic.Data.Models.Carma.Activity;
using Agency = GmWeb.Logic.Data.Models.Carma.Agency;
using ActivityCalendar = GmWeb.Logic.Data.Models.Carma.ActivityCalendar;
using Gender = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using EnumExtensions = GmWeb.Logic.Utility.Extensions.Enums.EnumExtensions;
using GmUserManager = GmWeb.Logic.Utility.Identity.GmUserManager;

namespace GmWeb.Tests.Api.Data
{
    public class DataEntities
    {
        public GmWebOptions WebOptions { get; }
        public string AdminEmail => $"admin@{WebOptions.BaseDomain}";
        public string AdminPassword { get; set; }
        public string MemberEmail => $"member@{WebOptions.BaseDomain}";
        public string MemberPassword { get; set; }

        public Agency Agency { get; set; }
        public GmIdentity AdminIdentity { get; set; }
        public User AdminStaffer { get; set; }
        public GmIdentity MemberIdentity { get; set; }
        public User MemberClient { get; set; }
        public ActivityCalendar ActivityCalendar { get; set; }
        public List<Activity> Activities { get; set; }
        public Note AdminNote { get; set; }
        public DataEntities(IOptions<GmWebOptions> webOptions)
        {
            this.WebOptions = webOptions.Value;
        }
    }
}
