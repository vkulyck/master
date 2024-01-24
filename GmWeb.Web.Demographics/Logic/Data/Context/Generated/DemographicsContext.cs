







using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Lookups;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Logic.Data.Models.Demographics;
namespace GmWeb.Web.Demographics.Logic.Data.Context
{
    public partial class DemographicsContext : BaseDataContext<DemographicsContext>
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCategory> ClientCategories { get; set; }
        public DbSet<ClientCategoryDate> ClientCategoryDates { get; set; }
        public DbSet<ClientServiceProfile> ClientServiceProfiles { get; set; }
        public DbSet<Ethnicity> Ethnicities { get; set; }
        public DbSet<ProfileCategory> ProfileCategories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkPlan> WorkPlans { get; set; }
        public DbSet<AssemblyDistrictShape> AssemblyDistrictShapes { get; set; }
        public DbSet<CensusTractShape> CensusTractShapes { get; set; }
        public DbSet<CongressionalDistrictShape> CongressionalDistrictShapes { get; set; }
        public DbSet<CountyShape> CountyShapes { get; set; }
        public DbSet<NeighborhoodShape> NeighborhoodShapes { get; set; }
        public DbSet<PrecinctShape> PrecinctShapes { get; set; }
        public DbSet<StateSenateDistrictShape> StateSenateDistrictShapes { get; set; }
        public DbSet<SupervisorDistrictShape> SupervisorDistrictShapes { get; set; }
        public DbSet<ZipcodeShape> ZipcodeShapes { get; set; }
        public DbSet<Bin> Bins { get; set; }
        public DbSet<BinValue> BinValues { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryValue> CategoryValues { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<LivingWageEstimate> LivingWageEstimates { get; set; }
        public DbSet<HUDIncomeLevel> HUDIncomeLevels { get; set; }

        protected void ConfigureGeneratedEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureGeneratedEntity<Activity>();
            modelBuilder.ConfigureGeneratedEntity<Agency>();
            modelBuilder.ConfigureGeneratedEntity<Client>();
            modelBuilder.ConfigureGeneratedEntity<ClientCategory>();
            modelBuilder.ConfigureGeneratedEntity<ClientCategoryDate>();
            modelBuilder.ConfigureGeneratedEntity<ClientServiceProfile>();
            modelBuilder.ConfigureGeneratedEntity<Ethnicity>();
            modelBuilder.ConfigureGeneratedEntity<ProfileCategory>();
            modelBuilder.ConfigureGeneratedEntity<Project>();
            modelBuilder.ConfigureGeneratedEntity<User>();
            modelBuilder.ConfigureGeneratedEntity<WorkPlan>();
            modelBuilder.ConfigureGeneratedEntity<AssemblyDistrictShape>();
            modelBuilder.ConfigureGeneratedEntity<CensusTractShape>();
            modelBuilder.ConfigureGeneratedEntity<CongressionalDistrictShape>();
            modelBuilder.ConfigureGeneratedEntity<CountyShape>();
            modelBuilder.ConfigureGeneratedEntity<NeighborhoodShape>();
            modelBuilder.ConfigureGeneratedEntity<PrecinctShape>();
            modelBuilder.ConfigureGeneratedEntity<StateSenateDistrictShape>();
            modelBuilder.ConfigureGeneratedEntity<SupervisorDistrictShape>();
            modelBuilder.ConfigureGeneratedEntity<ZipcodeShape>();
            modelBuilder.ConfigureGeneratedEntity<Bin>();
            modelBuilder.ConfigureGeneratedEntity<BinValue>();
            modelBuilder.ConfigureGeneratedEntity<Category>();
            modelBuilder.ConfigureGeneratedEntity<CategoryValue>();
            modelBuilder.ConfigureGeneratedEntity<Dataset>();
            modelBuilder.ConfigureGeneratedEntity<LivingWageEstimate>();
            modelBuilder.ConfigureGeneratedEntity<HUDIncomeLevel>();
        }
    }
}

