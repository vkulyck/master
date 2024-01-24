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
    public partial class AccountCache : BaseDataCache<AccountCache, AccountContext>
    {
        public AccountCache()
        {
            this.Initialize();
        }

        public AccountCache(AccountContext context) : base(context)
        {
            this.Initialize();
        }

        protected override void InitializeCollectionMap()
        {
		    this.CollectionMap[typeof(ClientAccount)] = () => this.ClientAccounts;
		    this.CollectionMap[typeof(ClientAccounts)] = () => this.ClientAccounts;

		    this.CollectionMap[typeof(UserAccount)] = () => this.UserAccounts;
		    this.CollectionMap[typeof(UserAccounts)] = () => this.UserAccounts;

        }

		private ClientAccounts _ClientAccounts;
		public ClientAccounts ClientAccounts => _ClientAccounts ?? (_ClientAccounts = new ClientAccounts(this));

		private UserAccounts _UserAccounts;
		public UserAccounts UserAccounts => _UserAccounts ?? (_UserAccounts = new UserAccounts(this));

    }
}
