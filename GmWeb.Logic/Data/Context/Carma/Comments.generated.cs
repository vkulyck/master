using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Comments : BaseDataCollection<Comment, Comments, CarmaCache, CarmaContext>
    {
        public Comments(CarmaCache cache) : base(cache) { }

        public override DbSet<Comment> EntitySet => this.DataContext.Comments;
	}
}
