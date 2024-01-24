using GmWeb.Logic.Data.Models.Carma;
namespace GmWeb.Web.Common.Auth
{
    public interface IJwtAuthResult
    {
        JwtAuthToken AuthToken { get; set; }
        JwtRefreshToken RefreshToken { get; set; }
        User User { get; }
        GmIdentity Identity { get; }
    }
}
