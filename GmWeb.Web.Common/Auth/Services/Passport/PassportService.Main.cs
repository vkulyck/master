using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Web.Common.Auth;
using GmWeb.Web.Common.Options;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthHeader = System.Net.Http.Headers.AuthenticationHeaderValue;
using HttpMethod = System.Net.Http.HttpMethod;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using StatusCode = System.Net.HttpStatusCode;
using GmWeb.Web.Common.Auth.Tokens;
using GmWeb.Web.Common.Auth.Services.Passport;
using CaseExtensions;


namespace GmWeb.Web.Common.Auth.Services.Passport;

public abstract partial class PassportService : IPassportService
{
    private static AnonymousPassportService _anon;
    public PassportService Anonymous => _anon;
    protected MappingFactory Mapper => MappingFactory.Instance;
    public JwtPassport Passport { get; protected set; }
    protected virtual bool EnableAnonymousSubservice => true;
    public virtual Uri BaseClientUri => this.WebOptions.Api.BaseUri;
    private readonly ILogger<PassportService> _logger;
    public virtual ILogger Logger => _logger;

    public GmWebOptions WebOptions { get; protected set; }
    public JwtAuthOptions JwtOptions { get; protected set; }

    public PassportService(IOptions<GmWebOptions> webOptions, IOptions<JwtAuthOptions> jwtOptions, ILoggerFactory factory)
    {
        this.WebOptions = webOptions.Value;
        this.JwtOptions = jwtOptions.Value;
        if (this.EnableAnonymousSubservice)
        {
            _anon = new AnonymousPassportService(webOptions, jwtOptions, factory);
        }
        _logger = factory.CreateLogger<PassportService>();
    }

    public async Task<JwtPassport> LoginAsync(JwtPassport passport)
    {
        var authToken = JwtAuthToken.Deserialize(passport.AuthToken, this.JwtOptions);
        if (authToken.IsValid)
        {
            this.Passport = passport;
            return passport;
        }
        var refreshToken = JwtRefreshToken.Deserialize(passport.RefreshToken, this.JwtOptions);
        if (refreshToken.IsValid)
        {
            this.Passport = await this.RenewPassportAsync(passport);
            return this.Passport;
        }
        return null;
    }

    protected async Task<TokenResult> RenewPassportAsync(JwtPassport passport = null)
    {
        if (passport != null)
            this.Passport = passport;
        var result = await this.PostAsync<TokenResult>("auth/retoken", refresh: true);
        return result;
    }

    public async Task LogoutAsync()
        => await this.PostAsync("auth/logout");

    protected static string InferEndpoint(string controller, string FunctionName)
        => $"{controller}/{InferActionName(FunctionName)}";
    protected static string InferEndpoint<TControllerType>(TControllerType controller, string FunctionName)
        where TControllerType : struct
        => $"{controller}/{InferActionName(FunctionName)}";

    protected static string InferActionName(string FunctionName)
    {
        if (FunctionName == null)
            throw new ArgumentNullException(nameof(FunctionName));
        if (string.IsNullOrWhiteSpace(FunctionName))
            return string.Empty;
        string @async = "Async";
        if (FunctionName.EndsWith(@async))
            FunctionName = FunctionName.Substring(0, FunctionName.Length - async.Length);
        var actionName = FunctionName.ToKebabCase();
        return actionName;
    }
}
