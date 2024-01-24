using System;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Web.Api.Models.Identity
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public Guid AccountID { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
