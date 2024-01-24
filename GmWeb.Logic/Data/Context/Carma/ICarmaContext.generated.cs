using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;
namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class CarmaContext : ICarmaContext { }

    public interface ICarmaContext : IBaseDataContext<ICarmaContext>
    {

        DbSet<ActivityCalendar> ActivityCalendars { get; }
        DbSet<Address> Addresses { get; }
        DbSet<Agency> Agencies { get; }
        DbSet<Comment> Comments { get; }
        DbSet<Note> Notes { get; }
        DbSet<Thread> Threads { get; }
        DbSet<User> Users { get; }
        DbSet<UserActivity> UserActivities { get; }
        DbSet<UserConfig> UserConfigs { get; }
    }

}
