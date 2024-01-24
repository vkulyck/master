using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace GmWeb.Web.Common.Identity
{
    public interface ISharedTokenGeneratorProvider<TUser>
        where TUser : class, IUser<string>
    {
        Task<string> ConfigureSharedSecret(UserManager<TUser, string> manager, TUser user);
    }
}