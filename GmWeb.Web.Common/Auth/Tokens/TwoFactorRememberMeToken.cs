using GmWeb.Logic.Utility.Extensions.Chronometry;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using GmWeb.Logic.Utility.Redis;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;


namespace GmWeb.Web.Common.Auth;

public struct TwoFactorRememberMeToken : IAccountToken
{
    public Guid AccountID { get; set; }

    public string TokenID => "tfa-rm";

    public TimeSpan Lifetime { get; set; }

    public string TokenType => null;
}
