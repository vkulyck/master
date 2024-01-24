using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class Users : BaseDataCollection<User, Users, CarmaCache, CarmaContext>
    {
        public Users(CarmaCache cache) : base(cache) { }

        public override DbSet<User> EntitySet => this.DataContext.Users;
	}
}
