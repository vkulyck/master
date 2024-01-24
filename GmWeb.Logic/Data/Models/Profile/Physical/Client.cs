using GmWeb.Logic.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("tblClient")]
    public class Client : BaseDataModel
    {
        [Key]
        public int ClientID { get; set; }
        public int? AgencyID { get; set; }
        [EncryptedStringColumn]
        public string FirstName { get; set; }
        [EncryptedStringColumn]
        public string LastName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Ethnicity { get; set; }
        public int? Age { get; set; }
        public string IncomeLevel { get; set; }
        [SqlDataType(System.Data.SqlDbType.VarChar)]
        [EncryptedDateTimeColumn]
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public int? HeadOfHouseholdType { get; set; }
        public int? NumberInFamily { get; set; }
        public int? NumberInHousehold { get; set; }
        [InverseProperty("Client")]
        public virtual ICollection<ClientCategory> ClientCategories { get; set; } = new List<ClientCategory>();
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [Column("Password")]
        public string PasswordHash { get; set; }
    }
}
