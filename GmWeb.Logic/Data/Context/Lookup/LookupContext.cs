using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Data.Context.Lookup
{
    public partial class LookupContext : BaseDataContext<LookupContext>
    {
        public LookupContext() { }
        public LookupContext(DbContextOptions<LookupContext> options) : base(options) { }
    }
}
