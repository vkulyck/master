using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.Shared
{
    public class Volunteer : BaseEntityViewModel
    {
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string CulturalIdentityID { get; set; }
        public string LanguageIDPrimary { get; set; }
        public string LanguageIDSecondary { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressLine2 { get; set; }

        public override void CopyRowData(DataRow row)
        {
            this.Birthday = row.ToString("Birthday");
            this.Gender = row.ToString("Gender").Replace("M", "Male").Replace("F", "Female").Replace("T", "Transgender").Replace("U", "Unidentified");
            this.Ethnicity = row.ToString("EthDescription");
            this.CulturalIdentityID = row.ToString("CulturalIdentityDescription");
            this.LanguageIDPrimary = row.ToString("LanguagePrimaryDescription");
            this.LanguageIDSecondary = row.ToString("LanguageSecondaryDescription");
            this.Email = row.ToString("Email");
            this.Phone = row.ToString("Phone");
            this.Address = row.ToString("AddressNo") + " " + row.ToString("AddressStreet") + " " + row.ToString("AddressStreetType") + ", " + row.ToString("City") + ", " + row.ToString("Zip");
            this.AddressLine2 = row.ToString("AddressLine2");
        }
    }
}