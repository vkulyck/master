using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public class ServiceLinkProcessData
    {
        public ServiceLinkStatus Status { get; set; }
        public int CategoryID { get; set; }
        public List<AgreementItem> PendingAgreements { get; set; }
        public bool Success { get; set; } = false;
    }
}