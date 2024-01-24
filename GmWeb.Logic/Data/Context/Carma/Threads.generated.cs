using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Threads : BaseDataCollection<Thread, Threads, CarmaCache, CarmaContext>
    {
        public Threads(CarmaCache cache) : base(cache) { }

        public override DbSet<Thread> EntitySet => this.DataContext.Threads;
	}
}
