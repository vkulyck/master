using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public enum ServiceLinkStatus
    {
        LinkSucceeded,
        ConsentRequired,
        ConsentAffirmed,
        ConsentDenied
    }
}