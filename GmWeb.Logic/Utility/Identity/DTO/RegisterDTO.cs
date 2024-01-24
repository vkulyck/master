using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Utility.Identity.DTO;
public class RegisterDTO
{
    public string UserName => this.Email;

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string FirstName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string LastName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public string BirthDate { get; set; }
}