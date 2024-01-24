﻿







using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Lookups;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Data.Models.Waitlists;
namespace GmWeb.Logic.Data.Context.Account
{
    public partial class AccountContext : BaseDataContext<AccountContext>
    {

        public DbSet<ClientAccount> ClientAccounts { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected void ConfigureGeneratedEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureGeneratedEntity<ClientAccount>();
            modelBuilder.ConfigureGeneratedEntity<UserAccount>();
        }
    }
}