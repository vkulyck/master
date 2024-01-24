using GmWeb.Web.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.JwtAuth;
public static class JwtAuthExtensions
{
    public static async Task SendFeedback(this JwtBearerChallengeContext context, ErrorResponse feedback)
    {
        await context.SendFeedback<JwtBearerOptions>(feedback);
        context.HandleResponse();
    }
    public static async Task SendFeedback<TOptions>(this BaseContext<TOptions> context, ErrorResponse feedback)
        where TOptions : AuthenticationSchemeOptions
    {
        context.Response.StatusCode = (int)feedback.StatusCode;
        await context.Response.WriteAsJsonAsync(feedback);
    }
    public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services)
        => services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddScheme<JwtAuthOptions, JwtAuthHandler>(JwtBearerDefaults.AuthenticationScheme, "JWT Auth", opts => { })
    ;
}
