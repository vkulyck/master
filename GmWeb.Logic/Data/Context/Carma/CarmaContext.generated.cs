







using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;
namespace GmWeb.Logic.Data.Context.Carma;


public abstract class BaseCarmaContext : BaseDataContext<CarmaContext>
{

    public DbSet<ActivityCalendar> ActivityCalendars { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Thread> Threads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<UserConfig> UserConfigs { get; set; }

    public BaseCarmaContext() { }
    public BaseCarmaContext(DbContextOptions<CarmaContext> options) : base(options) { }

    #region Before Creating

    protected virtual void BeforeModelCreating(ModelBuilder modelBuilder)
    {
        this.BeforeActivityCalendarsCreating(modelBuilder, this._configuration);
        this.BeforeAddressesCreating(modelBuilder, this._configuration);
        this.BeforeAgenciesCreating(modelBuilder, this._configuration);
        this.BeforeCommentsCreating(modelBuilder, this._configuration);
        this.BeforeNotesCreating(modelBuilder, this._configuration);
        this.BeforeThreadsCreating(modelBuilder, this._configuration);
        this.BeforeUsersCreating(modelBuilder, this._configuration);
        this.BeforeUserActivitiesCreating(modelBuilder, this._configuration);
        this.BeforeUserConfigsCreating(modelBuilder, this._configuration);
    }

    protected virtual EntityTypeBuilder<ActivityCalendar> BeforeActivityCalendarsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<ActivityCalendar>();
    protected virtual EntityTypeBuilder<Address> BeforeAddressesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Address>();
    protected virtual EntityTypeBuilder<Agency> BeforeAgenciesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Agency>();
    protected virtual EntityTypeBuilder<Comment> BeforeCommentsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Comment>();
    protected virtual EntityTypeBuilder<Note> BeforeNotesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Note>();
    protected virtual EntityTypeBuilder<Thread> BeforeThreadsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Thread>();
    protected virtual EntityTypeBuilder<User> BeforeUsersCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<User>();
    protected virtual EntityTypeBuilder<UserActivity> BeforeUserActivitiesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<UserActivity>();
    protected virtual EntityTypeBuilder<UserConfig> BeforeUserConfigsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<UserConfig>();
    #endregion

    #region On Creating

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.BeforeModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
        this.OnActivityCalendarsCreating(modelBuilder, this._configuration);
        this.OnAddressesCreating(modelBuilder, this._configuration);
        this.OnAgenciesCreating(modelBuilder, this._configuration);
        this.OnCommentsCreating(modelBuilder, this._configuration);
        this.OnNotesCreating(modelBuilder, this._configuration);
        this.OnThreadsCreating(modelBuilder, this._configuration);
        this.OnUsersCreating(modelBuilder, this._configuration);
        this.OnUserActivitiesCreating(modelBuilder, this._configuration);
        this.OnUserConfigsCreating(modelBuilder, this._configuration);

        this.AfterModelCreating(modelBuilder);
    }

    protected virtual EntityTypeBuilder<ActivityCalendar> OnActivityCalendarsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<ActivityCalendar>(config);
    protected virtual EntityTypeBuilder<Address> OnAddressesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<Address>(config);
    protected virtual EntityTypeBuilder<Agency> OnAgenciesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<Agency>(config);
    protected virtual EntityTypeBuilder<Comment> OnCommentsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<Comment>(config);
    protected virtual EntityTypeBuilder<Note> OnNotesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<Note>(config);
    protected virtual EntityTypeBuilder<Thread> OnThreadsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<Thread>(config);
    protected virtual EntityTypeBuilder<User> OnUsersCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<User>(config);
    protected virtual EntityTypeBuilder<UserActivity> OnUserActivitiesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<UserActivity>(config);
    protected virtual EntityTypeBuilder<UserConfig> OnUserConfigsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.ConfigureGeneratedEntity<UserConfig>(config);
    #endregion

    #region After Creating

    protected virtual void AfterModelCreating(ModelBuilder modelBuilder)
    {
        this.AfterActivityCalendarsCreating(modelBuilder, this._configuration);
        this.AfterAddressesCreating(modelBuilder, this._configuration);
        this.AfterAgenciesCreating(modelBuilder, this._configuration);
        this.AfterCommentsCreating(modelBuilder, this._configuration);
        this.AfterNotesCreating(modelBuilder, this._configuration);
        this.AfterThreadsCreating(modelBuilder, this._configuration);
        this.AfterUsersCreating(modelBuilder, this._configuration);
        this.AfterUserActivitiesCreating(modelBuilder, this._configuration);
        this.AfterUserConfigsCreating(modelBuilder, this._configuration);
    }

    protected virtual EntityTypeBuilder<ActivityCalendar> AfterActivityCalendarsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<ActivityCalendar>();
    protected virtual EntityTypeBuilder<Address> AfterAddressesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Address>();
    protected virtual EntityTypeBuilder<Agency> AfterAgenciesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Agency>();
    protected virtual EntityTypeBuilder<Comment> AfterCommentsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Comment>();
    protected virtual EntityTypeBuilder<Note> AfterNotesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Note>();
    protected virtual EntityTypeBuilder<Thread> AfterThreadsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<Thread>();
    protected virtual EntityTypeBuilder<User> AfterUsersCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<User>();
    protected virtual EntityTypeBuilder<UserActivity> AfterUserActivitiesCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<UserActivity>();
    protected virtual EntityTypeBuilder<UserConfig> AfterUserConfigsCreating(ModelBuilder modelBuilder, IConfiguration config)
        => modelBuilder.Entity<UserConfig>();

    #endregion

}
