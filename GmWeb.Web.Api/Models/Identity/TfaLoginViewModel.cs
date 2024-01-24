using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;

namespace GmWeb.Web.Api.Models.Identity
{
    public class TfaLoginViewModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public TwoFactorProviderType Provider { get; set; }
        public bool IsPersistent { get; set; }
        public bool RememberClient { get; set; }
    }
}