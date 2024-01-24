// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using GmWeb.Web.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.JwtAuth;

/// <summary>
/// An <see cref="JwtBearerHandler{TOptions}"/> that can perform JWT-bearer based authentication.
/// This implementation is based on the JwtBearerHandler implementation from Microsoft's .NET Core/ASP.NET Core repositories on Github.
/// See: https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/JwtBearer/src/JwtBearerHandler.cs
/// </summary>
public class JwtAuthHandler : JwtBearerHandler<JwtAuthOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="JwtAuthHandler"/>.
    /// </summary>
    /// <inheritdoc />
    public JwtAuthHandler(IOptionsMonitor<JwtAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }

    /// <summary>
    /// The handler calls methods on the events which give the application control at certain points where processing is occurring.
    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    /// </summary>
    protected new JwtAuthEvents Events
    {
        get => (JwtAuthEvents)base.Events;
        set => base.Events = value;
    }

    /// <inheritdoc />
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new JwtAuthEvents());
}
