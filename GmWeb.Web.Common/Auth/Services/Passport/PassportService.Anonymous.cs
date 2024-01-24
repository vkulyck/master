using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.Passport;
public partial class AnonymousPassportService : PassportService
{
    private readonly ILogger<AnonymousPassportService> _logger;
    public override ILogger Logger => _logger;
    protected override bool EnableAnonymousSubservice => false;
    public AnonymousPassportService(IOptions<GmWebOptions> webOptions, IOptions<JwtAuthOptions> jwtOptions, ILoggerFactory factory)
        : base(webOptions, jwtOptions, factory)
    { 
        _logger = factory.CreateLogger<AnonymousPassportService>();
    }

    public override Task<HttpClient> CreateHttpClient(bool refresh = false) => Task.FromResult(new HttpClient());
}