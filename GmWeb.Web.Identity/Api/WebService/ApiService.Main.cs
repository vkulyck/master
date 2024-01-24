using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using GmWeb.Web.Common.Auth.Services.Passport;

namespace GmWeb.Web.Identity.Api.WebService;
public partial class ApiService : PassportService
{
    private readonly ILogger<ApiService> _logger;
    public override ILogger Logger => _logger;
    public ApiService(IOptions<GmWebOptions> webOptions, IOptions<JwtAuthOptions> jwtOptions, ILoggerFactory factory)
        : base(webOptions, jwtOptions, factory)
    {
        _logger = factory.CreateLogger<ApiService>();
    }
    protected static string AcctEndpoint(string FunctionName)
    => InferEndpoint(ControllerType.Account, FunctionName);
    protected static string AuthEndpoint(string FunctionName)
        => InferEndpoint(ControllerType.Auth, FunctionName);
    protected static string ClmEndpoint(string FunctionName)
        => InferEndpoint(ControllerType.Claim, FunctionName);
    protected static string MgrEndpoint(string FunctionName)
        => InferEndpoint(ControllerType.Manage, FunctionName);
}
