using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class UserActivities : BaseDataCollection<UserActivity, UserActivities, CarmaCache, CarmaContext>
    {
        public UserActivities(CarmaCache cache) : base(cache) { }

        public override DbSet<UserActivity> EntitySet => this.DataContext.UserActivities;
	}
}
