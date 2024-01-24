using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using IAuthorizationRequirement = Microsoft.AspNetCore.Authorization.IAuthorizationRequirement;
using AuthorizationFailureReason = Microsoft.AspNetCore.Authorization.AuthorizationFailureReason;

namespace GmWeb.Logic.Utility.Web;

public partial class ErrorResponse
{
    public static ErrorResponse BadRequest(Exception ex, params string[] messages)
        => new ErrorResponse(HttpStatusCode.BadRequest, ex, messages);
    public static ErrorResponse BadRequest(string message)
        => new ErrorResponse(HttpStatusCode.BadRequest, message);
    public static ErrorResponse BadRequest(Exception ex)
        => new ErrorResponse(HttpStatusCode.BadRequest, ex);
    public static ErrorResponse BadRequest(params string[] errors)
        => new ErrorResponse(HttpStatusCode.BadRequest, errors);
    public static ErrorResponse BadRequest(IEnumerable<string> errors)
        => new ErrorResponse(HttpStatusCode.BadRequest, errors);
    public static ErrorResponse BadRequest(ModelStateDictionary modelState)
        => new ErrorResponse(HttpStatusCode.BadRequest, modelState);
    public static ErrorResponse BadRequest(IdentityResult idResult)
        => new ErrorResponse(HttpStatusCode.BadRequest, idResult);
    public static ErrorResponse BadRequest(IEnumerable<IAuthorizationRequirement> failures)
        => new ErrorResponse(HttpStatusCode.BadRequest, failures);
    public static ErrorResponse BadRequest(IEnumerable<AuthorizationFailureReason> failures)
        => new ErrorResponse(HttpStatusCode.BadRequest, failures);
    public static ErrorResponse NotFound(Exception ex, params string[] messages)
        => new ErrorResponse(HttpStatusCode.NotFound, ex, messages);
    public static ErrorResponse NotFound(string message)
        => new ErrorResponse(HttpStatusCode.NotFound, message);
    public static ErrorResponse NotFound(Exception ex)
        => new ErrorResponse(HttpStatusCode.NotFound, ex);
    public static ErrorResponse NotFound(params string[] errors)
        => new ErrorResponse(HttpStatusCode.NotFound, errors);
    public static ErrorResponse NotFound(IEnumerable<string> errors)
        => new ErrorResponse(HttpStatusCode.NotFound, errors);
    public static ErrorResponse NotFound(ModelStateDictionary modelState)
        => new ErrorResponse(HttpStatusCode.NotFound, modelState);
    public static ErrorResponse NotFound(IdentityResult idResult)
        => new ErrorResponse(HttpStatusCode.NotFound, idResult);
    public static ErrorResponse NotFound(IEnumerable<IAuthorizationRequirement> failures)
        => new ErrorResponse(HttpStatusCode.NotFound, failures);
    public static ErrorResponse NotFound(IEnumerable<AuthorizationFailureReason> failures)
        => new ErrorResponse(HttpStatusCode.NotFound, failures);
    public static ErrorResponse Unauthorized(Exception ex, params string[] messages)
        => new ErrorResponse(HttpStatusCode.Unauthorized, ex, messages);
    public static ErrorResponse Unauthorized(string message)
        => new ErrorResponse(HttpStatusCode.Unauthorized, message);
    public static ErrorResponse Unauthorized(Exception ex)
        => new ErrorResponse(HttpStatusCode.Unauthorized, ex);
    public static ErrorResponse Unauthorized(params string[] errors)
        => new ErrorResponse(HttpStatusCode.Unauthorized, errors);
    public static ErrorResponse Unauthorized(IEnumerable<string> errors)
        => new ErrorResponse(HttpStatusCode.Unauthorized, errors);
    public static ErrorResponse Unauthorized(ModelStateDictionary modelState)
        => new ErrorResponse(HttpStatusCode.Unauthorized, modelState);
    public static ErrorResponse Unauthorized(IdentityResult idResult)
        => new ErrorResponse(HttpStatusCode.Unauthorized, idResult);
    public static ErrorResponse Unauthorized(IEnumerable<IAuthorizationRequirement> failures)
        => new ErrorResponse(HttpStatusCode.Unauthorized, failures);
    public static ErrorResponse Unauthorized(IEnumerable<AuthorizationFailureReason> failures)
        => new ErrorResponse(HttpStatusCode.Unauthorized, failures);
    public static ErrorResponse Forbidden(Exception ex, params string[] messages)
        => new ErrorResponse(HttpStatusCode.Forbidden, ex, messages);
    public static ErrorResponse Forbidden(string message)
        => new ErrorResponse(HttpStatusCode.Forbidden, message);
    public static ErrorResponse Forbidden(Exception ex)
        => new ErrorResponse(HttpStatusCode.Forbidden, ex);
    public static ErrorResponse Forbidden(params string[] errors)
        => new ErrorResponse(HttpStatusCode.Forbidden, errors);
    public static ErrorResponse Forbidden(IEnumerable<string> errors)
        => new ErrorResponse(HttpStatusCode.Forbidden, errors);
    public static ErrorResponse Forbidden(ModelStateDictionary modelState)
        => new ErrorResponse(HttpStatusCode.Forbidden, modelState);
    public static ErrorResponse Forbidden(IdentityResult idResult)
        => new ErrorResponse(HttpStatusCode.Forbidden, idResult);
    public static ErrorResponse Forbidden(IEnumerable<IAuthorizationRequirement> failures)
        => new ErrorResponse(HttpStatusCode.Forbidden, failures);
    public static ErrorResponse Forbidden(IEnumerable<AuthorizationFailureReason> failures)
        => new ErrorResponse(HttpStatusCode.Forbidden, failures);
    public static ErrorResponse InternalServerError(Exception ex, params string[] messages)
        => new ErrorResponse(HttpStatusCode.InternalServerError, ex, messages);
    public static ErrorResponse InternalServerError(string message)
        => new ErrorResponse(HttpStatusCode.InternalServerError, message);
    public static ErrorResponse InternalServerError(Exception ex)
        => new ErrorResponse(HttpStatusCode.InternalServerError, ex);
    public static ErrorResponse InternalServerError(params string[] errors)
        => new ErrorResponse(HttpStatusCode.InternalServerError, errors);
    public static ErrorResponse InternalServerError(IEnumerable<string> errors)
        => new ErrorResponse(HttpStatusCode.InternalServerError, errors);
    public static ErrorResponse InternalServerError(ModelStateDictionary modelState)
        => new ErrorResponse(HttpStatusCode.InternalServerError, modelState);
    public static ErrorResponse InternalServerError(IdentityResult idResult)
        => new ErrorResponse(HttpStatusCode.InternalServerError, idResult);
    public static ErrorResponse InternalServerError(IEnumerable<IAuthorizationRequirement> failures)
        => new ErrorResponse(HttpStatusCode.InternalServerError, failures);
    public static ErrorResponse InternalServerError(IEnumerable<AuthorizationFailureReason> failures)
        => new ErrorResponse(HttpStatusCode.InternalServerError, failures);
}
