using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Addresses : BaseDataCollection<Address, Addresses, CarmaCache, CarmaContext>
    {
        public Addresses(CarmaCache cache) : base(cache) { }

        public override DbSet<Address> EntitySet => this.DataContext.Addresses;
	}
}
