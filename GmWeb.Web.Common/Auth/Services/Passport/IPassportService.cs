using System.Threading.Tasks;
using GmWeb.Web.Common.Auth.Tokens;

namespace GmWeb.Web.Common.Auth.Services.Passport;

public interface IPassportService
{
    Task<JwtPassport> LoginAsync(JwtPassport passport);
}
