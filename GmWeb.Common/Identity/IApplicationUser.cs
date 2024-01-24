using System;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Common
{
    public interface IApplicationUser
    {

        int UserID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        AppIdentityType IdentityType { get; set; }
        DateTime? AuthenticationDate { get; set; }
        DateTime? RegistrationDate { get; set; }
        int? AgencyID { get; set; }
        [Phone]
        string PhoneNumber { get; set; }

        bool IsUser();
        bool IsClient();
    }
}
