using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GmWeb.Logic.Interfaces;
using System.Data;
using System.Linq;
using GmWeb.Common;
using GmWeb.Logic.Data.Models.Profile;
using System.Threading.Tasks;
using System.Security.Claims;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Common.Identity
{
    // You can add profile data for the user by adding more properties to your AppIdentity class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class GmIdentity : IdentityUser, IApplicationUser
    {
        public GmIdentity()
        {
            this.LockoutEnabled = true;
        }
        public override string UserName 
        {
            get => this.Email;
            set => this.Email = value; 
        }
        [NotMapped]
        public virtual int UserID { get; set; }
        [NotMapped]
        public virtual int ClientID { get; set; }
        [NotMapped]
        public virtual AppIdentityType IdentityType { get; set; }
        [NotMapped]
        public DateTime? AuthenticationDate { get; set; }
        [NotMapped]
        public DateTime? RegistrationDate { get; set; }
        [NotMapped]
        public int? AgencyID
        {
            get => this.Account?.AgencyID;
            set
            {
                if (this.Account == null)
                    return;
                this.Account.AgencyID = value;
            }
        }
        [NotMapped]
        public string FirstName
        {
            get => this.Account?.FirstName;
            set
            {
                if (this.Account == null)
                    return;
                this.Account.FirstName = value;
            }
        }
        [NotMapped]
        public string LastName
        {
            get => this.Account?.LastName;
            set
            {
                if (this.Account == null)
                    return;
                this.Account.LastName = value;
            }
        }
        [NotMapped]
        public string Phone
        {
            get => this.PhoneNumber;
            set => this.PhoneNumber = value;
        }

        public override string SecurityStamp { get; set; } = Guid.NewGuid().ToString("D");
        public string GetInformation(InformationType infoType)
        {
            var propertyName = infoType.ToString();
            var property = this.GetType().GetProperty(propertyName);
            var value = property.GetValue(this)?.ToString();
            return value;
        }
        public bool IsUser() => !this.IsClient();
        public bool IsClient() => this.IdentityType == AppIdentityType.Client;

        [NotMapped]
        public BaseAccount Account { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync<TUser>(UserManager<TUser,string> manager)
            where TUser : GmIdentity
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync((TUser)this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
