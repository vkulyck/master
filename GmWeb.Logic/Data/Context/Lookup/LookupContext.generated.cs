







using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Shared;
using GmWeb.Logic.Utility.Extensions;
using Newtonsoft.Json;
namespace GmWeb.Logic.Data.Context.Lookup
{
    public partial class LookupContext : BaseDataContext<LookupContext>
    {

        public DbSet<LookupOption> LookupOptions { get; set; }

        protected void ConfigureGeneratedEntities(ModelBuilder modelBuilder)
        {
        }
    }
}
