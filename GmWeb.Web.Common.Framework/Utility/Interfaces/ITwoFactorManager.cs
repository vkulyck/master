using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using GmWeb.Web.Common.Identity;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Common.Utility
{
    public interface ITwoFactorManager<TUser> : IDisposable
        where TUser : class, IUser<string>
    {
        IDictionary<InformationType, TotpSecurityStampBasedTokenProvider<TUser,string>> TwoFactorMethods { get; }
        Task<IDictionary<InformationType, bool>> GetProviderConfigurationAsync(TUser user);

        Task<string> ConfigureTwoFactorSecret(string provider, string userId, InformationType infoType);
    }
}
