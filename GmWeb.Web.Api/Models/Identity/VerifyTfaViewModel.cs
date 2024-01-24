using System;
using System.ComponentModel.DataAnnotations;
using TwoFactorProviderType = GmWeb.Logic.Enums.TwoFactorProviderType;

namespace GmWeb.Web.Api.Models.Identity
{
    public class VerifyTfaViewModel
    {
        [Required]
        public Guid AccountID { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public TwoFactorProviderType Provider { get; set; }
    }
}