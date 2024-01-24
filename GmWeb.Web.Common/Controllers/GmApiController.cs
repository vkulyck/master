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

namespace GmWeb.Web.Api.Controllers;

[Produces("application/json")]
[Route(ApiVersion + "/[controller]/[action]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + BypassAuthExtensions.AuthenticationScheme)]
public class GmApiController : GmController
{
    public const string ApiVersion = "v1";

    protected GmApiController(UserManager<GmIdentity> manager) : base(manager) { }
}