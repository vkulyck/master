using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using OtpSharp;
using Base32;
using GmWeb.Web.Common.Utility;
using GmWeb.Logic.Enums;
using BaseProviders = Microsoft.AspNet.Identity;

namespace GmWeb.Web.Common.Identity
{
    public class EmailTokenProvider<TUser> : BaseProviders.EmailTokenProvider<TUser>
        where TUser : class, IUser<string>
    {
    }
}