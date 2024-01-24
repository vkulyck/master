using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AuthorizationFailureReason = Microsoft.AspNetCore.Authorization.AuthorizationFailureReason;
using EnumExtensions = GmWeb.Logic.Utility.Extensions.Enums.EnumExtensions;
using IAuthorizationRequirement = Microsoft.AspNetCore.Authorization.IAuthorizationRequirement;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using ModelStateDictionary = Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;
using ApiIdentityResult = GmWeb.Logic.Utility.Identity.Results.ApiIdentityResult;

namespace GmWeb.Logic.Utility.Web;
public partial class ErrorResponse
{
    public static readonly HashSet<HttpStatusCode> SuccessCodes = new();
    [JsonProperty("success")]
    public bool Success { get; set; }
    [JsonProperty("errors")]
    public List<RequestError> Errors { get; set; }
    [JsonProperty("status")]
    public HttpStatusCode StatusCode { get; set; }
    [JsonProperty("challenge")]
    public string Challenge { get; set; }
    static ErrorResponse()
    {
        var codeModels = EnumExtensions.GetEnumViewModels<HttpStatusCode>();
        SuccessCodes = codeModels.Where(x => x.ID < 300).Select(x => x.Value).ToHashSet();
    }

    #region Constructors
    public ErrorResponse() : this(HttpStatusCode.OK) { }
    public ErrorResponse(HttpStatusCode code)
    {
        this.StatusCode = code;
        this.Success = SuccessCodes.Contains(code);
    }
    public ApiIdentityResult AsIdentityResult()
    {
        var result = new ApiIdentityResult
        {
            Succeeded = !this.Errors.Any(),
            Errors = this.Errors.Select(x => new ApiIdentityResult.ApiError(x.Message, this.StatusCode.ToString())).ToList()
        };
        return result;
    }
    public static implicit operator ApiIdentityResult(ErrorResponse response)
    {
        return response.AsIdentityResult();
    }
    public ErrorResponse(HttpStatusCode code, Exception ex, params string[] messages) : this(code)
    {
        this.Errors = new List<RequestError> { new RequestError(ex) };
        foreach (string message in messages)
            this.Errors.Add(new RequestError(message));
    }
    public ErrorResponse(HttpStatusCode code, string message) : this(code)
    {
        this.Errors = new List<RequestError> { new RequestError(message) };
    }
    public ErrorResponse(HttpStatusCode code, IEnumerable<string> messages) : this(code)
    {
        this.Errors = messages.Select(x => new RequestError(x)).ToList();
    }
    public ErrorResponse(HttpStatusCode code, Exception ex) : this(code)
    {
        this.Errors = new List<RequestError> { new RequestError(ex) };
    }
    public ErrorResponse(HttpStatusCode code, ModelStateDictionary modelState) : this(code)
    {
        this.Success = false;
        this.Errors = modelState.Values.SelectMany(x => x.Errors).Select(x => new RequestError(x.ErrorMessage)).ToList();
    }
    public ErrorResponse(HttpStatusCode code, IdentityResult idResult) : this(code)
    {
        this.Success = false;
        this.Errors = idResult.Errors.Select(x => x.Description).Select(x => new RequestError(x)).ToList();
    }
    public ErrorResponse(HttpStatusCode code, IEnumerable<IAuthorizationRequirement> failures) : this(code)
    {
        this.Success = false;
        this.Errors = failures.Select(x => new RequestError(x.ToString())).ToList();
    }
    public ErrorResponse(HttpStatusCode code, IEnumerable<AuthorizationFailureReason> failures) : this(code)
    {
        this.Success = false;
        this.Errors = failures.Select(x => new RequestError(x.Message)).ToList();
    }
    #endregion
}