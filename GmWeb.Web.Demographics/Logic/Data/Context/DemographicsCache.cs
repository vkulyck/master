using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Context.DataInitializers;

namespace GmWeb.Web.Demographics.Logic.Data.Context
{
    public partial class DemographicsCache : BaseDataCache<DemographicsCache,DemographicsContext>
    {
        public override void Initialize()
        {
            DemographicInitializer.Seed(this);
        }
    }
}
