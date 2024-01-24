using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GmWeb.Common;
using GmWeb.Logic.Enums;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace GmWeb.Web.Identity.Models
{
    public class IndexViewModel
    {
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Login Status")]
        public bool IsLoggedIn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }
    public class RequestPasswordResetViewModel
    {
        [DataType(SystemDataType.EmailAddress)]
        [Required(ErrorMessage = "A valid email address is required.")]
        [DisplayName("Email Address")]
        public string Email { get; set; }
        public bool IsEmailSent { get; set; } = false;
    }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [DataType(SystemDataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public AppIdentityType IdentityType { get; set; }
    }

    public class MigrateViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public AppIdentityType IdentityType { get; set; }
    }    
    
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(SystemDataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(SystemDataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string ReturnUrl { get; set; }
        public AppIdentityType IdentityType { get; set; }
    }

    public class RequestTokenViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsEmailSent { get; set; } = false;
    }

    public class TestRegistrationViewModel
    {
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Login Status")]
        public bool IsLoggedIn { get; set; }
        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(SystemDataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(SystemDataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public enum ManageMessageId
    {
        UpdatePhoneSuccess,
        RemovePhoneSuccess,       
        UpdateEmailSuccess,
        UpdatePasswordSuccess,
        UpdateYubikeySuccess,
        ManageTfaSuccess,
        Error
    }
}