using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Logic.Data.Models.Lookups;
using GmWeb.Logic.Data.Models.Profile;
using Microsoft.EntityFrameworkCore;
namespace GmWeb.Logic.Data.Context
{
    public interface IDemographicsContext : IBaseDataContext<IDemographicsContext>
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
        DbSet<AssemblyDistrictShape> AssemblyDistrictShapes { get; }
        DbSet<CensusTractShape> CensusTractShapes { get; }
        DbSet<CongressionalDistrictShape> CongressionalDistrictShapes { get; }
        DbSet<CountyShape> CountyShapes { get; }
        DbSet<NeighborhoodShape> NeighborhoodShapes { get; }
        DbSet<PrecinctShape> PrecinctShapes { get; }
        DbSet<StateSenateDistrictShape> StateSenateDistrictShapes { get; }
        DbSet<SupervisorDistrictShape> SupervisorDistrictShapes { get; }
        DbSet<ZipcodeShape> ZipcodeShapes { get; }
        DbSet<Bin> Bins { get; }
        DbSet<BinValue> BinValues { get; }
        DbSet<Category> Categories { get; }
        DbSet<CategoryValue> CategoryValues { get; }
        DbSet<Dataset> Datasets { get; }
        DbSet<LivingWageEstimate> LivingWageEstimates { get; }
        DbSet<HUDIncomeLevel> HUDIncomeLevels { get; }
    }
}
