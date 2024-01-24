using System;

namespace GmWeb.Web.Common.Auth.Tokens;
public class JwtPassport
{
    public string AuthToken { get; set; }
    public string RefreshToken { get; set; }
    public Guid AccountID { get; set; }
    public Guid LookupID { get; set; }
    public int UserID { get; set; }
    public DateTime UtcExpiration { get; set; }
    public DateTime Expiration => this.UtcExpiration.ToLocalTime();
}