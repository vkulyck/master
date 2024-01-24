using GmWeb.Logic.Data.Context.Identity;
using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Data.Context.Account
{
    public partial class AccountContext : BaseDataContext<AccountContext>
    {
        public AccountContext() { }
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }

        public LegacyRegistration LegacyRegistration => LegacyRegistration.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ConfigureGeneratedEntities(modelBuilder);
        }
    }
}
