using System;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Utility.Identity.DTO;

public class ChangeEmailDTO
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }
}