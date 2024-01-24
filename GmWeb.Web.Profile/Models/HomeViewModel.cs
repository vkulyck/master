using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models
{
    public class HomeViewModel : GmWeb.Web.Common.Models.BasePageViewModel
    {
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string LanguageIDSecondary { get; set; }
        public string Address { get; set; }
        public string CulturalIdentityID { get; set; }
        public string AddressLine2 { get; set; }
        public string LanguageIDPrimary { get; set; }
        public string Phone { get; set; }
        public string ServicesCountHeader { get; set; }

    }
}