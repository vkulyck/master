using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GmWeb.Web.Common.Auth.Services.Passport;

namespace GmWeb.Web.RHI.Utility;
public partial class ApiService : PassportService
{
    private readonly ILogger<ApiService> _logger;
    public override ILogger Logger => _logger;
    public ApiService(IOptions<GmWebOptions> webOptions, IOptions<JwtAuthOptions> jwtOptions, ILoggerFactory factory)
        : base(webOptions, jwtOptions, factory)
    {
        _logger = factory.CreateLogger<ApiService>();
    }
    protected static string IntakeEndpoint(string FunctionName)
    => InferEndpoint("Intake", FunctionName);
}