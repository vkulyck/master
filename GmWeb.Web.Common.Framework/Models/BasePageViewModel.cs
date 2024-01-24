using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Common.Models
{
    public class BasePageViewModel
    {
        public string ClientFullName => FullName;
        public string FullName { get; set; }
        public int ClientID { get; set; }
        public string AgencyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? AgencyID { get; set; }
        public int AgencyIDParent { get; set; }
        public int ParentAgencyID { get => this.AgencyIDParent; set => this.AgencyIDParent = value; }
    }
}