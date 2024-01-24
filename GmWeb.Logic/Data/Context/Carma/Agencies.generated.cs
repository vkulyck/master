using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Agencies : BaseDataCollection<Agency, Agencies, CarmaCache, CarmaContext>
    {
        public Agencies(CarmaCache cache) : base(cache) { }

        public override DbSet<Agency> EntitySet => this.DataContext.Agencies;
	}
}
