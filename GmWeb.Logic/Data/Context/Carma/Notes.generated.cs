using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Notes : BaseDataCollection<Note, Notes, CarmaCache, CarmaContext>
    {
        public Notes(CarmaCache cache) : base(cache) { }

        public override DbSet<Note> EntitySet => this.DataContext.Notes;
	}
}
