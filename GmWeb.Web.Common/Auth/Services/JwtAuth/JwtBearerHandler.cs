﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.JwtAuth;

/// <summary>
/// An <see cref="AuthenticationHandler{TOptions}"/> that can perform JWT-bearer based authentication.
/// This implementation is based on the JwtBearerHandler implementation from Microsoft's .NET Core/ASP.NET Core repositories on Github.
/// See: https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/JwtBearer/src/JwtBearerHandler.cs
/// </summary>
public class JwtBearerHandler<TOptions> : AuthenticationHandler<TOptions>
    where TOptions : JwtBearerOptions, new()
{
    private OpenIdConnectConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of <see cref="JwtBearerHandler"/>.
    /// </summary>
    /// <inheritdoc />
    public JwtBearerHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }

    /// <summary>
    /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    /// </summary>
    protected new JwtBearerEvents Events
    {
        get => (JwtBearerEvents)base.Events!;
        set => base.Events = value;
    }

    /// <inheritdoc />
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new JwtBearerEvents());

    /// <summary>
    /// Searches the 'Authorization' header for a 'Bearer' token. If the 'Bearer' token is found, it is validated using <see cref="TokenValidationParameters"/> set in the options.
    /// </summary>
    /// <returns></returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string token = null;
        try
        {
            // Give application opportunity to find from a different location, adjust, or reject token
            var messageReceivedContext = new MessageReceivedContext(this.Context, this.Scheme, this.Options);

            // event can set the token
            await this.Events.MessageReceived(messageReceivedContext);
            if (messageReceivedContext.Result != null)
            {
                return messageReceivedContext.Result;
            }

            // If application retrieved token from somewhere else, use that.
            token = messageReceivedContext.Token;

            if (string.IsNullOrEmpty(token))
            {
                string authorization = this.Request.Headers.Authorization.ToString();

                // If no authorization header found, nothing to process further
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.NoResult();
                }

                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.NoResult();
                }
            }

            if (this._configuration == null && this.Options.ConfigurationManager != null)
            {
                this._configuration = await this.Options.ConfigurationManager.GetConfigurationAsync(this.Context.RequestAborted);
            }

            var validationParameters = this.Options.TokenValidationParameters.Clone();
            if (this._configuration != null)
            {
                string[] issuers = new[] { this._configuration.Issuer };
                validationParameters.ValidIssuers = validationParameters.ValidIssuers?.Concat(issuers) ?? issuers;

                validationParameters.IssuerSigningKeys = validationParameters.IssuerSigningKeys?.Concat(this._configuration.SigningKeys)
                    ?? this._configuration.SigningKeys;
            }

            List<Exception> validationFailures = null;
            SecurityToken validatedToken = null;
            foreach (var validator in this.Options.SecurityTokenValidators)
            {
                if (validator.CanReadToken(token))
                {
                    ClaimsPrincipal principal;
                    try
                    {
                        principal = validator.ValidateToken(token, validationParameters, out validatedToken);
                    }
                    catch (Exception ex)
                    {
                        //Logger.TokenValidationFailed(ex);

                        // Refresh the configuration for exceptions that may be caused by key rollovers. The user can also request a refresh in the event.
                        if (this.Options.RefreshOnIssuerKeyNotFound && this.Options.ConfigurationManager != null
                            && ex is SecurityTokenSignatureKeyNotFoundException)
                        {
                            this.Options.ConfigurationManager.RequestRefresh();
                        }

                        if (validationFailures == null)
                        {
                            validationFailures = new List<Exception>(1);
                        }
                        validationFailures.Add(ex);
                        continue;
                    }

                    //Logger.TokenValidationSucceeded();

                    var tokenValidatedContext = new TokenValidatedContext(this.Context, this.Scheme, this.Options)
                    {
                        Principal = principal,
                        SecurityToken = validatedToken
                    };

                    tokenValidatedContext.Properties.ExpiresUtc = GetSafeDateTime(validatedToken.ValidTo);
                    tokenValidatedContext.Properties.IssuedUtc = GetSafeDateTime(validatedToken.ValidFrom);

                    await this.Events.TokenValidated(tokenValidatedContext);
                    if (tokenValidatedContext.Result != null)
                    {
                        return tokenValidatedContext.Result;
                    }

                    if (this.Options.SaveToken)
                    {
                        tokenValidatedContext.Properties.StoreTokens(new[]
                        {
                                new AuthenticationToken { Name = "access_token", Value = token }
                            });
                    }

                    tokenValidatedContext.Success();
                    return tokenValidatedContext.Result!;
                }
            }

            if (validationFailures != null)
            {
                var authenticationFailedContext = new AuthenticationFailedContext(this.Context, this.Scheme, this.Options)
                {
                    Exception = (validationFailures.Count == 1) ? validationFailures[0] : new AggregateException(validationFailures)
                };

                await this.Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                return AuthenticateResult.Fail(authenticationFailedContext.Exception);
            }

            return AuthenticateResult.Fail("No SecurityTokenValidator available for token.");
        }
        catch (Exception ex)
        {
            //Logger.ErrorProcessingMessage(ex);

            var authenticationFailedContext = new AuthenticationFailedContext(this.Context, this.Scheme, this.Options)
            {
                Exception = ex
            };

            await this.Events.AuthenticationFailed(authenticationFailedContext);
            if (authenticationFailedContext.Result != null)
            {
                return authenticationFailedContext.Result;
            }

            throw;
        }
    }

    private static DateTime? GetSafeDateTime(DateTime dateTime)
    {
        // Assigning DateTime.MinValue or default(DateTime) to a DateTimeOffset when in a UTC+X timezone will throw
        // Since we don't really care about DateTime.MinValue in this case let's just set the field to null
        if (dateTime == DateTime.MinValue)
        {
            return null;
        }
        return dateTime;
    }

    /// <inheritdoc />
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await this.HandleAuthenticateOnceSafeAsync();
        var eventContext = new JwtBearerChallengeContext(this.Context, this.Scheme, this.Options, properties)
        {
            AuthenticateFailure = authResult?.Failure
        };

        // Avoid returning error=invalid_token if the error is not caused by an authentication failure (e.g missing token).
        if (this.Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
        {
            eventContext.Error = "invalid_token";
            eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
        }

        await this.Events.Challenge(eventContext);
        if (eventContext.Handled)
        {
            return;
        }

        this.Response.StatusCode = 401;

        if (string.IsNullOrEmpty(eventContext.Error) &&
            string.IsNullOrEmpty(eventContext.ErrorDescription) &&
            string.IsNullOrEmpty(eventContext.ErrorUri))
        {
            this.Response.Headers.Append(HeaderNames.WWWAuthenticate, this.Options.Challenge);
        }
        else
        {
            // https://tools.ietf.org/html/rfc6750#section-3.1
            // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
            var builder = new StringBuilder(this.Options.Challenge);
            if (this.Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(" error=\"");
                builder.Append(eventContext.Error);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
            {
                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    builder.Append(',');
                }

                builder.Append(" error_description=\"");
                builder.Append(eventContext.ErrorDescription);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                if (!string.IsNullOrEmpty(eventContext.Error) ||
                    !string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    builder.Append(',');
                }

                builder.Append(" error_uri=\"");
                builder.Append(eventContext.ErrorUri);
                builder.Append('\"');
            }

            this.Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
    }

    /// <inheritdoc />
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var forbiddenContext = new ForbiddenContext(this.Context, this.Scheme, this.Options);
        this.Response.StatusCode = 403;
        return this.Events.Forbidden(forbiddenContext);
    }

    private static string CreateErrorDescription(Exception authFailure)
    {
        IReadOnlyCollection<Exception> exceptions;
        if (authFailure is AggregateException agEx)
        {
            exceptions = agEx.InnerExceptions;
        }
        else
        {
            exceptions = new[] { authFailure };
        }

        var messages = new List<string>(exceptions.Count);

        foreach (var ex in exceptions)
        {
            // Order sensitive, some of these exceptions derive from others
            // and we want to display the most specific message possible.
            switch (ex)
            {
                case SecurityTokenInvalidAudienceException stia:
                    messages.Add($"The audience '{stia.InvalidAudience ?? "(null)"}' is invalid");
                    break;
                case SecurityTokenInvalidIssuerException stii:
                    messages.Add($"The issuer '{stii.InvalidIssuer ?? "(null)"}' is invalid");
                    break;
                case SecurityTokenNoExpirationException _:
                    messages.Add("The token has no expiration");
                    break;
                case SecurityTokenInvalidLifetimeException stil:
                    messages.Add("The token lifetime is invalid; NotBefore: "
                        + $"'{stil.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'"
                        + $", Expires: '{stil.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)"}'");
                    break;
                case SecurityTokenNotYetValidException stnyv:
                    messages.Add($"The token is not valid before '{stnyv.NotBefore.ToString(CultureInfo.InvariantCulture)}'");
                    break;
                case SecurityTokenExpiredException ste:
                    messages.Add($"The token expired at '{ste.Expires.ToString(CultureInfo.InvariantCulture)}'");
                    break;
                case SecurityTokenSignatureKeyNotFoundException _:
                    messages.Add("The signature key was not found");
                    break;
                case SecurityTokenInvalidSignatureException _:
                    messages.Add("The signature is invalid");
                    break;
            }
        }

        return string.Join("; ", messages);
    }
}