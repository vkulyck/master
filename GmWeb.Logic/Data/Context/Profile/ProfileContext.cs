using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Profile.Logical;
using Microsoft.EntityFrameworkCore;


namespace GmWeb.Logic.Data.Context.Profile
{
    public partial class ProfileContext : BaseDataContext<ProfileContext>, IWaitlistContext
    {
        public ProfileContext() { }
        public ProfileContext(DbContextOptions<ProfileContext> options) : base(options) { }

        /// <summary>
        /// An Client data model with extended properties from tblClientServicesProfile
        /// </summary>
        public DbSet<ExtendedClient> ExtendedClients { get; set; }

        // TODO: Setup query for tracts
        public DbSet<CensusTract> CensusTracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ConfigureGeneratedEntities(modelBuilder);
            this.createProfileDatasets(modelBuilder);
        }

        protected void createProfileDatasets(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(new ExtendedClientConfiguration());
    }
}
