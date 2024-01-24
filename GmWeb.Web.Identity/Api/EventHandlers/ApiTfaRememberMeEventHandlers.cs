using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GmWeb.Web.Common.Auth;
using GmWeb.Web.Common.Auth.Services.Passport;
using JwtPassport = GmWeb.Web.Common.Auth.Tokens.JwtPassport;
using GmWeb.Web.Common.Auth.EventHandlers;

namespace GmWeb.Web.Identity.Api.EventHandlers;
public class ApiTfaRememberMeEventHandlers : TfaRememberMeEventHandlers
{
    public ApiTfaRememberMeEventHandlers(ApiUserManager manager, SignInManager<GmIdentity> signin, IHttpContextAccessor accessor, IOptions<JwtAuthOptions> jwtAuthOptions)
        : base(manager, signin, accessor, manager.Api, jwtAuthOptions)
    { }
}
