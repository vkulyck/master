using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class UserConfigs : BaseDataCollection<UserConfig, UserConfigs, CarmaCache, CarmaContext>
    {
        public UserConfigs(CarmaCache cache) : base(cache) { }

        public override DbSet<UserConfig> EntitySet => this.DataContext.UserConfigs;
	}
}
