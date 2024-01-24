using System;
using System.Security.Claims;
using GmWeb.Web.Common.Auth.Services.BypassAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using ApiIdentityResult = GmWeb.Logic.Utility.Identity.Results.ApiIdentityResult;

namespace GmWeb.Web.Common.Controllers;

public static class GmControllerExtensions
{
    public static BadRequestObjectResult AsIdentityResult(this BadRequestObjectResult result)
    {
        var errorResponse = result.Value as ErrorResponse;
        var idResult = new BadRequestObjectResult(errorResponse?.AsIdentityResult());
        return idResult;
    }
}

public class GmController : Controller
{
    protected MappingFactory Mapper => MappingFactory.Instance;

    private readonly UserManager<GmIdentity> _manager;
    protected UserManager<GmIdentity> Manager => this._manager;

    protected GmController(UserManager<GmIdentity> manager)
    {
        this._manager = manager;
    }

    protected string GetUserId()
    {
        var claim = this.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return default;
        var claimValue = claim.Value;
        if (string.IsNullOrWhiteSpace(claimValue))
            return default;
        return claimValue;
    }

    protected async Task<GmIdentity> GetUserAsync()
    {
        return await this.Manager.GetUserAsync(this.HttpContext.User);
    }

        protected ObjectResult ServerError(Exception ex)
            => this.StatusCode(500, ErrorResponse.InternalServerError(ex));
        protected OkObjectResult Success() => this.Ok(new { success = true });
        protected OkObjectResult Success(object data) => this.Ok(data);
        protected IActionResult Infer(IdentityResult result)
        {
            if (result.Succeeded)
                return this.Success(result);
            return this.BadRequest(result.Errors.Select(x => x.Description));
        }
        protected IActionResult BadRequest(IEnumerable<string> messages) => base.BadRequest(ErrorResponse.BadRequest(messages));
        protected BadRequestObjectResult BadRequest(string message) => base.BadRequest(ErrorResponse.BadRequest(message));
        protected BadRequestObjectResult MissingRouteId() => this.BadRequest("You must specify an id in the request route.");
        protected BadRequestObjectResult MissingQueryFilters() => this.BadRequest("You must specify the filter parameter(s) in the request query.");
        protected BadRequestObjectResult MissingPostModel() => this.BadRequest("Model data must be provided within the request body.");
        protected BadRequestObjectResult DataNotFound() => this.BadRequest("The requested data could not be retrieved.");
        protected BadRequestObjectResult MissingListFilters() => this.BadRequest("List filters must be provided within the request query string.");
        protected BadRequestObjectResult ModelStateErrors() => this.BadRequest(ErrorResponse.BadRequest(this.ModelState));
        protected BadRequestObjectResult IdentityErrors(IdentityResult result) => this.BadRequest(ErrorResponse.BadRequest(result));
        protected BadRequestObjectResult IdentityErrors(string Description, HttpStatusCode Code = HttpStatusCode.BadRequest)
            => this.BadRequest(IdentityResult.Failed(new IdentityError
            {
                Code = Code.ToString(),
                Description = Description 
            }));
}