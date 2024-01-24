using System;
using System.Security.Claims;
using GmWeb.Web.Common.Controllers;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Controllers;

[Authorize(AuthenticationSchemes = GmWeb.Common.Identity.IdentityConstants.ApplicationScheme + "," + BypassAuthExtensions.AuthenticationScheme)]
public class GmAppController : GmController
{
    protected GmAppController(UserManager<GmIdentity> manager) : base(manager) { }
}