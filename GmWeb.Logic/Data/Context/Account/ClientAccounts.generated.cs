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
    public partial class ClientAccounts : BaseDataCollection<ClientAccount, ClientAccounts, AccountCache, AccountContext>
    {
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ClientAccounts(AccountCache cache) : base(cache) { }

        public override DbSet<ClientAccount> EntitySet => this.DataContext.ClientAccounts;
	}
}
