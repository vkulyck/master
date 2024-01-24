using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma;

public partial class CarmaCache : BaseDataCache<CarmaCache, CarmaContext>
{
    public CarmaCache()
    {
        this.Initialize();
    }

    public CarmaCache(CarmaContext context) : base(context)
    {
        this.Initialize();
    }

    protected override void InitializeCollectionMap()
    {
        this.CollectionMap[typeof(ActivityCalendar)] = () => this.ActivityCalendars;
        this.CollectionMap[typeof(ActivityCalendars)] = () => this.ActivityCalendars;

        this.CollectionMap[typeof(Address)] = () => this.Addresses;
        this.CollectionMap[typeof(Addresses)] = () => this.Addresses;

        this.CollectionMap[typeof(Agency)] = () => this.Agencies;
        this.CollectionMap[typeof(Agencies)] = () => this.Agencies;

        this.CollectionMap[typeof(Comment)] = () => this.Comments;
        this.CollectionMap[typeof(Comments)] = () => this.Comments;

        this.CollectionMap[typeof(Note)] = () => this.Notes;
        this.CollectionMap[typeof(Notes)] = () => this.Notes;

        this.CollectionMap[typeof(Thread)] = () => this.Threads;
        this.CollectionMap[typeof(Threads)] = () => this.Threads;

        this.CollectionMap[typeof(User)] = () => this.Users;
        this.CollectionMap[typeof(Users)] = () => this.Users;

        this.CollectionMap[typeof(UserActivity)] = () => this.UserActivities;
        this.CollectionMap[typeof(UserActivities)] = () => this.UserActivities;

        this.CollectionMap[typeof(UserConfig)] = () => this.UserConfigs;
        this.CollectionMap[typeof(UserConfigs)] = () => this.UserConfigs;

    }

    private ActivityCalendars _ActivityCalendars;
    public ActivityCalendars ActivityCalendars => _ActivityCalendars ?? (_ActivityCalendars = new ActivityCalendars(this));

    private Addresses _Addresses;
    public Addresses Addresses => _Addresses ?? (_Addresses = new Addresses(this));

    private Agencies _Agencies;
    public Agencies Agencies => _Agencies ?? (_Agencies = new Agencies(this));

    private Comments _Comments;
    public Comments Comments => _Comments ?? (_Comments = new Comments(this));

    private Notes _Notes;
    public Notes Notes => _Notes ?? (_Notes = new Notes(this));

    private Threads _Threads;
    public Threads Threads => _Threads ?? (_Threads = new Threads(this));

    private Users _Users;
    public Users Users => _Users ?? (_Users = new Users(this));

    private UserActivities _UserActivities;
    public UserActivities UserActivities => _UserActivities ?? (_UserActivities = new UserActivities(this));

    private UserConfigs _UserConfigs;
    public UserConfigs UserConfigs => _UserConfigs ?? (_UserConfigs = new UserConfigs(this));

}
