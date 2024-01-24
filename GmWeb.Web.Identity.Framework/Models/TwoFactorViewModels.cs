using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GmWeb.Common;
using GmWeb.Logic.Enums;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace GmWeb.Web.Identity.Models
{
    public enum ManagementTask
    {
        Reset,
        Update,
        Verify,
        Disable
    }
    public enum TaskResult
    {
        Successful,
        Failed
    }
    public class TwoFactorDetailsViewModel
    {
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public IDictionary<InformationType, bool> ProviderConfiguration { get; set; } = new Dictionary<InformationType, bool>();
        public InformationType InformationType { get; set; }
        public ManagementTask ManagementTask { get; set; }
        public TaskResult TaskResult { get; set; }
    }
    public class TwoFactorConfigurationViewModel
    {
        [DataType(SystemDataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [StringLength(32, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(SystemDataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(SystemDataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(SystemDataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [DataType(SystemDataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Verification Code")]
        public string Token { get; set; }
        public string SecretKey { get; set; }
        virtual public InformationType InformationType { get; set; }
        public ManagementTask ManagementTask { get; set; }

        public string SelectedProvider
        {
            get => this.InformationType.ToString();
            set => this.InformationType = (InformationType)(Enum.TryParse(value, out InformationType result) ? result : InformationType.Disabled);
        }
        public string ReturnUrl { get; set; }

        public bool IsTwoFactor() => this.InformationType.IsTwoFactor();
    }

    public class PasswordConfigurationViewModel : TwoFactorConfigurationViewModel
    {
        public override InformationType InformationType
        {
            get => InformationType.Password;
            set { }
        }
    }

    namespace ProviderConfiguration
    {
        public class EmailConfigurationViewModel : TwoFactorConfigurationViewModel { }
        public class GoogleAuthenticatorConfigurationViewModel : TwoFactorConfigurationViewModel { }
        public class PhoneConfigurationViewModel : TwoFactorConfigurationViewModel { }
        public class YubikeyConfigurationViewModel : TwoFactorConfigurationViewModel { }
        public class FidoConfigurationViewModel : TwoFactorConfigurationViewModel { }
    }

    public class TwoFactorSelectionViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class TwoFactorVerificationViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

}