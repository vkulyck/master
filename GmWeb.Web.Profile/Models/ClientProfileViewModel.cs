using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Profile.Models.Shared;

namespace GmWeb.Web.Profile.Models
{
    public class ClientProfileViewModel : Common.Models.BasePageViewModel
    {
        public string ClientSupplementalData { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int ClientGroupID { get; set; }
        public Volunteer Volunteer { get; set; }
        public bool EnablePrevButton => this.PrevClientDataTypeID.HasValue;
        public bool EnableNextButton => this.NextClientDataTypeID.HasValue;
        public int? PrevClientDataTypeID { get; set; }
        public int ClientDataTypeID { get; set; }
        public int? NextClientDataTypeID { get; set; }
        public bool ProfileOnly { get; set; }
    }
}