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
    public partial class DemographicsCache : BaseDataCache<DemographicsCache, DemographicsContext>
    {
        public DemographicsCache()
        {
            this.Initialize();
        }

        protected override void InitializeCollectionMap()
        {
		    this.CollectionMap[typeof(Activity)] = () => this.Activities;
		    this.CollectionMap[typeof(Activities)] = () => this.Activities;

		    this.CollectionMap[typeof(Agency)] = () => this.Agencies;
		    this.CollectionMap[typeof(Agencies)] = () => this.Agencies;

		    this.CollectionMap[typeof(Client)] = () => this.Clients;
		    this.CollectionMap[typeof(Clients)] = () => this.Clients;

		    this.CollectionMap[typeof(ClientCategory)] = () => this.ClientCategories;
		    this.CollectionMap[typeof(ClientCategories)] = () => this.ClientCategories;

		    this.CollectionMap[typeof(ClientCategoryDate)] = () => this.ClientCategoryDates;
		    this.CollectionMap[typeof(ClientCategoryDates)] = () => this.ClientCategoryDates;

		    this.CollectionMap[typeof(ClientServiceProfile)] = () => this.ClientServiceProfiles;
		    this.CollectionMap[typeof(ClientServiceProfiles)] = () => this.ClientServiceProfiles;

		    this.CollectionMap[typeof(Ethnicity)] = () => this.Ethnicities;
		    this.CollectionMap[typeof(Ethnicities)] = () => this.Ethnicities;

		    this.CollectionMap[typeof(ProfileCategory)] = () => this.ProfileCategories;
		    this.CollectionMap[typeof(ProfileCategories)] = () => this.ProfileCategories;

		    this.CollectionMap[typeof(Project)] = () => this.Projects;
		    this.CollectionMap[typeof(Projects)] = () => this.Projects;

		    this.CollectionMap[typeof(User)] = () => this.Users;
		    this.CollectionMap[typeof(Users)] = () => this.Users;

		    this.CollectionMap[typeof(WorkPlan)] = () => this.WorkPlans;
		    this.CollectionMap[typeof(WorkPlans)] = () => this.WorkPlans;

		    this.CollectionMap[typeof(AssemblyDistrictShape)] = () => this.AssemblyDistrictShapes;
		    this.CollectionMap[typeof(AssemblyDistrictShapes)] = () => this.AssemblyDistrictShapes;

		    this.CollectionMap[typeof(CensusTractShape)] = () => this.CensusTractShapes;
		    this.CollectionMap[typeof(CensusTractShapes)] = () => this.CensusTractShapes;

		    this.CollectionMap[typeof(CongressionalDistrictShape)] = () => this.CongressionalDistrictShapes;
		    this.CollectionMap[typeof(CongressionalDistrictShapes)] = () => this.CongressionalDistrictShapes;

		    this.CollectionMap[typeof(CountyShape)] = () => this.CountyShapes;
		    this.CollectionMap[typeof(CountyShapes)] = () => this.CountyShapes;

		    this.CollectionMap[typeof(NeighborhoodShape)] = () => this.NeighborhoodShapes;
		    this.CollectionMap[typeof(NeighborhoodShapes)] = () => this.NeighborhoodShapes;

		    this.CollectionMap[typeof(PrecinctShape)] = () => this.PrecinctShapes;
		    this.CollectionMap[typeof(PrecinctShapes)] = () => this.PrecinctShapes;

		    this.CollectionMap[typeof(StateSenateDistrictShape)] = () => this.StateSenateDistrictShapes;
		    this.CollectionMap[typeof(StateSenateDistrictShapes)] = () => this.StateSenateDistrictShapes;

		    this.CollectionMap[typeof(SupervisorDistrictShape)] = () => this.SupervisorDistrictShapes;
		    this.CollectionMap[typeof(SupervisorDistrictShapes)] = () => this.SupervisorDistrictShapes;

		    this.CollectionMap[typeof(ZipcodeShape)] = () => this.ZipcodeShapes;
		    this.CollectionMap[typeof(ZipcodeShapes)] = () => this.ZipcodeShapes;

		    this.CollectionMap[typeof(Bin)] = () => this.Bins;
		    this.CollectionMap[typeof(Bins)] = () => this.Bins;

		    this.CollectionMap[typeof(BinValue)] = () => this.BinValues;
		    this.CollectionMap[typeof(BinValues)] = () => this.BinValues;

		    this.CollectionMap[typeof(Category)] = () => this.Categories;
		    this.CollectionMap[typeof(Categories)] = () => this.Categories;

		    this.CollectionMap[typeof(CategoryValue)] = () => this.CategoryValues;
		    this.CollectionMap[typeof(CategoryValues)] = () => this.CategoryValues;

		    this.CollectionMap[typeof(Dataset)] = () => this.Datasets;
		    this.CollectionMap[typeof(Datasets)] = () => this.Datasets;

		    this.CollectionMap[typeof(LivingWageEstimate)] = () => this.LivingWageEstimates;
		    this.CollectionMap[typeof(LivingWageEstimates)] = () => this.LivingWageEstimates;

		    this.CollectionMap[typeof(HUDIncomeLevel)] = () => this.HUDIncomeLevels;
		    this.CollectionMap[typeof(HUDIncomeLevels)] = () => this.HUDIncomeLevels;

        }

		private Activities _Activities;
		public Activities Activities => _Activities ?? (_Activities = new Activities(this));

		private Agencies _Agencies;
		public Agencies Agencies => _Agencies ?? (_Agencies = new Agencies(this));

		private Clients _Clients;
		public Clients Clients => _Clients ?? (_Clients = new Clients(this));

		private ClientCategories _ClientCategories;
		public ClientCategories ClientCategories => _ClientCategories ?? (_ClientCategories = new ClientCategories(this));

		private ClientCategoryDates _ClientCategoryDates;
		public ClientCategoryDates ClientCategoryDates => _ClientCategoryDates ?? (_ClientCategoryDates = new ClientCategoryDates(this));

		private ClientServiceProfiles _ClientServiceProfiles;
		public ClientServiceProfiles ClientServiceProfiles => _ClientServiceProfiles ?? (_ClientServiceProfiles = new ClientServiceProfiles(this));

		private Ethnicities _Ethnicities;
		public Ethnicities Ethnicities => _Ethnicities ?? (_Ethnicities = new Ethnicities(this));

		private ProfileCategories _ProfileCategories;
		public ProfileCategories ProfileCategories => _ProfileCategories ?? (_ProfileCategories = new ProfileCategories(this));

		private Projects _Projects;
		public Projects Projects => _Projects ?? (_Projects = new Projects(this));

		private Users _Users;
		public Users Users => _Users ?? (_Users = new Users(this));

		private WorkPlans _WorkPlans;
		public WorkPlans WorkPlans => _WorkPlans ?? (_WorkPlans = new WorkPlans(this));

		private AssemblyDistrictShapes _AssemblyDistrictShapes;
		public AssemblyDistrictShapes AssemblyDistrictShapes => _AssemblyDistrictShapes ?? (_AssemblyDistrictShapes = new AssemblyDistrictShapes(this));

		private CensusTractShapes _CensusTractShapes;
		public CensusTractShapes CensusTractShapes => _CensusTractShapes ?? (_CensusTractShapes = new CensusTractShapes(this));

		private CongressionalDistrictShapes _CongressionalDistrictShapes;
		public CongressionalDistrictShapes CongressionalDistrictShapes => _CongressionalDistrictShapes ?? (_CongressionalDistrictShapes = new CongressionalDistrictShapes(this));

		private CountyShapes _CountyShapes;
		public CountyShapes CountyShapes => _CountyShapes ?? (_CountyShapes = new CountyShapes(this));

		private NeighborhoodShapes _NeighborhoodShapes;
		public NeighborhoodShapes NeighborhoodShapes => _NeighborhoodShapes ?? (_NeighborhoodShapes = new NeighborhoodShapes(this));

		private PrecinctShapes _PrecinctShapes;
		public PrecinctShapes PrecinctShapes => _PrecinctShapes ?? (_PrecinctShapes = new PrecinctShapes(this));

		private StateSenateDistrictShapes _StateSenateDistrictShapes;
		public StateSenateDistrictShapes StateSenateDistrictShapes => _StateSenateDistrictShapes ?? (_StateSenateDistrictShapes = new StateSenateDistrictShapes(this));

		private SupervisorDistrictShapes _SupervisorDistrictShapes;
		public SupervisorDistrictShapes SupervisorDistrictShapes => _SupervisorDistrictShapes ?? (_SupervisorDistrictShapes = new SupervisorDistrictShapes(this));

		private ZipcodeShapes _ZipcodeShapes;
		public ZipcodeShapes ZipcodeShapes => _ZipcodeShapes ?? (_ZipcodeShapes = new ZipcodeShapes(this));

		private Bins _Bins;
		public Bins Bins => _Bins ?? (_Bins = new Bins(this));

		private BinValues _BinValues;
		public BinValues BinValues => _BinValues ?? (_BinValues = new BinValues(this));

		private Categories _Categories;
		public Categories Categories => _Categories ?? (_Categories = new Categories(this));

		private CategoryValues _CategoryValues;
		public CategoryValues CategoryValues => _CategoryValues ?? (_CategoryValues = new CategoryValues(this));

		private Datasets _Datasets;
		public Datasets Datasets => _Datasets ?? (_Datasets = new Datasets(this));

		private LivingWageEstimates _LivingWageEstimates;
		public LivingWageEstimates LivingWageEstimates => _LivingWageEstimates ?? (_LivingWageEstimates = new LivingWageEstimates(this));

		private HUDIncomeLevels _HUDIncomeLevels;
		public HUDIncomeLevels HUDIncomeLevels => _HUDIncomeLevels ?? (_HUDIncomeLevels = new HUDIncomeLevels(this));

    }
}
