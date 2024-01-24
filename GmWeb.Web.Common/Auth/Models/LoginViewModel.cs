using System.ComponentModel.DataAnnotations;

namespace GmWeb.Web.Common.Auth.Models;
public partial class LoginViewModel
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public bool IsPersistent { get; set; } = false;
    public bool LockoutOnFailure { get; set; } = false;
    public LoginViewModel()
        : this(Email: null, Password: null)
    { }
    public LoginViewModel(string Email, string Password)
        : this(Email, Password, IsPersistent: false)
    { }
    public LoginViewModel(string Email, string Password, bool IsPersistent)
        : this(Email, Password, IsPersistent: IsPersistent, LockoutOnFailure: true)
    { }
    public LoginViewModel(string Email, string Password, bool IsPersistent, bool LockoutOnFailure)
    {
        this.Email = Email;
        this.Password = Password;
        this.IsPersistent = IsPersistent;
        this.LockoutOnFailure = LockoutOnFailure;
    }
}