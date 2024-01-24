using System.ComponentModel.DataAnnotations;

namespace GmWeb.Web.Api.Models.Identity
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
