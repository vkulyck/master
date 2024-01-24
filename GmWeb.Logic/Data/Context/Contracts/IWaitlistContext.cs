using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Waitlists;
using Microsoft.EntityFrameworkCore;
namespace GmWeb.Logic.Data.Context
{
    public interface IWaitlistContext : IBaseDataContext<IWaitlistContext>
    {
        DbSet<Activity> Activities { get; }
        DbSet<Agency> Agencies { get; }
        DbSet<Client> Clients { get; }
        DbSet<ClientCategory> ClientCategories { get; }
        DbSet<ClientCategoryDate> ClientCategoryDates { get; }
        DbSet<ClientServiceProfile> ClientServiceProfiles { get; }
        DbSet<Ethnicity> Ethnicities { get; }
        DbSet<ProfileCategory> ProfileCategories { get; }
        DbSet<Project> Projects { get; }
        DbSet<User> Users { get; }
        DbSet<WorkPlan> WorkPlans { get; }
        DbSet<DataSource> DataSources { get; }
        DbSet<Flow> Flows { get; }
        DbSet<FlowStep> FlowSteps { get; }
        DbSet<ExtendedClient> ExtendedClients { get; }
    }
}
