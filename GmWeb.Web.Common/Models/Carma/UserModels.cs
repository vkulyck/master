using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GmWeb.Logic.Utility.Extensions.Collections;
using GenderType = GmWeb.Logic.Enums.Gender;
using PrimaryLanguages = GmWeb.Logic.Services.Datasets.Languages.PrimaryLanguages;
using UserRoleType = GmWeb.Logic.Enums.UserRole;
using IPerson = GmWeb.Logic.Interfaces.IPerson;
using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

namespace GmWeb.Web.Common.Models.Carma
{
    public class UserUpsertDTO
    {
        public GenderType? Gender { get; set; }
        public UserRoleType? UserRole { get; set; }
        public string LanguageCode { get; set; }
        public bool? IsStarred { get; set; }
    }
    public class UserInsertDTO : UserUpsertDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
    }
    public class UserPasswordInsertDTO : UserInsertDTO
    {
        [Required]
        public string Password { get; set; }
        public string RoleId { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
    }
    public class UserUpdateDTO : UserUpsertDTO, IIntegerKeyModel
    {
        int IIntegerKeyModel.PrimaryKey => this.UserID;
        [Required]
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IntakeData IntakeData { get; set; }
    }

    public class UserDTO : IPerson, IIntegerKeyModel
    {
        int IIntegerKeyModel.PrimaryKey => this.UserID;
        public int UserID { get; set; }
        public Guid AccountID { get; set; }
        public Guid LookupID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string FullName => ((IPerson)this).GetFullName();
        public string Email { get; set; }
        public string SecurityStamp { get; set; }
        public GenderType Gender { get; set; } = GenderType.Unspecified;
        public UserRoleType UserRole { get; set; } = UserRoleType.Client;
        public string LanguageCode { get; set; } = PrimaryLanguages.English.Value;
        public bool? IsStarred { get; set; }
        public string ProfilePhotoUrl => $"~/img/profile/photos/{this.LookupID}.jpg";
    }
}
