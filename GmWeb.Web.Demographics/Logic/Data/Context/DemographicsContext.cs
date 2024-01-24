using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using GmWeb.Web.Demographics.Logic.DataModels;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Context;

namespace GmWeb.Web.Demographics.Logic.Data.Context
{
    public partial class DemographicsContext : BaseDataContext<DemographicsContext>, IDemographicsContext
    {
        public DemographicsContext() { }
        public DemographicsContext(DbContextOptions options) : base(options) { }

        public DbQuery<ClientMarker> ClientMarkers { get; set; }

        // Program Charts
        public DbQuery<ParticipantCategoryMonth> ParticipantCategoryMonths { get; set; }
        public DbQuery<ClientEthnicity> ClientEthnicities { get; set; }
        public DbQuery<ClientIncomeLevel> ClientIncomeLevels { get; set; }
        public DbQuery<ClientAge> ClientAges { get; set; }
        public DbQuery<ClientHoHGender> ClientHoHGenders { get; set; }
        public DbQuery<CityIncomeLevel> CityIncomeLevels { get; set; }
        public DbQuery<ClientWageGap> ClientWageGaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ConfigureGeneratedEntities(modelBuilder);
            this.createRegions(modelBuilder);
            this.createChartQueries(modelBuilder);
        }

        protected void createRegions(ModelBuilder modelBuilder)
        {
            modelBuilder.Query<ClientMarker>().ToQuery(() =>
                from c in this.Clients.AsQueryable()
                where c.Latitude != null && c.Longitude != null
                select new ClientMarker(c)
            );
        }

        protected void createChartQueries(ModelBuilder modelBuilder)
        {
            #region LINQ CLIENT DETAILS
            /*
            modelBuilder.Query<ParticipantCategoryMonth>().ToQuery(() =>
                from c in this.Clients
                join cc in this.ClientCategories on c.ClientID equals cc.ClientID
                join cat in this.Categories on cc.ClientCategoryID equals cat.CategoryID
                join ccd in this.ClientCategoryDates on cc.ClientCategoryID equals ccd.ClientCategoryID
                join wp in this.WorkPlans on cat.WorkPlanID equals wp.Work_ID
                join act in this.Activities on wp.ActivityID equals act.ActivityID
                join p in this.Projects on wp.WorkProjectID equals p.ProjectID
                join a in this.Agencies on p.AgencyID equals a.AgencyID
                where a.Name != "Test Agency"
                where ccd.Assigned == true
                select new ParticipantCategoryMonth
                {
                    ClientID = c.ClientID,
                    ClientCategoryID = cc.ClientCategoryID,
                    ClientCategoryDateID = ccd.ClientCategoryDateID,
                    ActivityID = act.ActivityID,
                    AgencyID = a.AgencyID,
                    AgencyName = a.Name,
                    ProjectYear = p.ProjectYear,
                    Program = p.Program,
                    WorkActivityType = wp.WorkActivityType,
                    ActivityDescription = act.ActivityDescription,
                    Assigned = ccd.Assigned,
                    HUDCode = act.HUDCode,
                    ScheduledDate = ccd.ScheduledDate,
                    NumberInFamily = c.NumberInFamily,
                    NumberInHousehold = c.NumberInHousehold,
                    IncomeLevel = ClientIncomeLevel.ParseIncomeLevel(c.IncomeLevel),
                    RawIncomeLevel = c.IncomeLevel
                }
            );
            */
            #endregion
            modelBuilder.Query<ParticipantCategoryMonth>().ToView("viewClientDetails", "dmo");
            modelBuilder.Query<ClientEthnicity>().ToView("viewClientDetails", "dmo");
            modelBuilder.Query<ClientIncomeLevel>().ToView("viewClientDetails", "dmo");
            modelBuilder.Query<ClientAge>().ToView("viewClientDetails", "dmo");
            modelBuilder.Query<ClientHoHGender>().ToView("viewClientDetails", "dmo");
            modelBuilder.Query<CityIncomeLevel>().ToView("viewCityIncomeLevels", "dmo");
            modelBuilder.Query<ClientWageGap>().ToView("viewLivingWageGaps", "dmo");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if(!builder.IsConfigured)
            {
                var connString = System.Configuration.ConfigurationManager.ConnectionStrings["CURRENT_INSTANCE_DB"].ConnectionString;
                builder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(connString, x => x.UseNetTopologySuite())
                ;
            }
        }
    }
}
