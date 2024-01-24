using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.JwtAuth;
public class JwtAuthEvents : JwtBearerEvents
{
    protected JwtAuthTokenService TokenService { get; private set; }
    protected virtual void ConfigureServices<TOptions>(BaseContext<TOptions> context)
        where TOptions : AuthenticationSchemeOptions => this.TokenService = context.HttpContext.RequestServices.GetService<JwtAuthTokenService>();
    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        this.ConfigureServices(context);
        context.Response.Headers.WWWAuthenticate = context.Options.Challenge;
        ErrorResponse feedback;
        if (context.AuthenticateFailure == null)
            feedback = ErrorResponse.Unauthorized("Client request lacks authorized credentials.");
        else
            feedback = ErrorResponse.Unauthorized(context.AuthenticateFailure, "Client request lacks authorized credentials.");
        await context.SendFeedback(feedback);
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        this.ConfigureServices(context);
        var token = context.SecurityToken as JwtSecurityToken;
        if (token == null)
            context.Fail("The request could not be authorized because the supplied access token was not properly formatted.");
        var response = await this.TokenService.ValidateRequestTokenAsync(token);
        if (response.IsRevoked)
            context.Fail("The request is not authorized because the supplied access token is no longer valid.");
    }
}
