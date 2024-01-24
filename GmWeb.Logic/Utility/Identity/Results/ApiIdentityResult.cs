using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace GmWeb.Logic.Utility.Identity.Results;

public class ApiIdentityResult
{
    public class ApiError
    {
        public string StackTrace { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public ApiError() : this(default, default) { }
        public ApiError(string message) : this(message, default) { }
        public ApiError(string message, string code)
        {
            this.Description = message;
            this.Code = code;
        }
    }
    public bool Succeeded { get; set; }
    public List<ApiError> Errors { get; set; }

    public static implicit operator IdentityResult(ApiIdentityResult apiResult)
    {
        if (apiResult.Succeeded)
            return IdentityResult.Success;
        if (apiResult.Errors == null)
            return IdentityResult.Failed();
        var idErrors = apiResult.Errors.Select(x => new IdentityError
        {
            Code = x.Code,
            Description = x.Description
        }).ToArray();
        return IdentityResult.Failed(idErrors);
    }
}