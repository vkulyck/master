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
    public partial class CongressionalDistrictShapes : BaseDataCollection<CongressionalDistrictShape, CongressionalDistrictShapes, DemographicsCache, DemographicsContext>
    {
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CongressionalDistrictShapes(DemographicsCache cache) : base(cache) { }

        public override DbSet<CongressionalDistrictShape> EntitySet => this.DataContext.CongressionalDistrictShapes;
	}
}
