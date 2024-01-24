using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Requests;

public class JwtRefreshRequest
{
    [JsonPropertyName(JwtRefreshToken.DataSourceKey)]
    public string RefreshToken { get; set; }
}
