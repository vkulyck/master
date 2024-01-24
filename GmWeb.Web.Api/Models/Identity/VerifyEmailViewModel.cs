using System;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Web.Api.Models.Identity
{
    public class VerifyEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
