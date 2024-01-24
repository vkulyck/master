using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Profile.Models.Waitlist;

namespace GmWeb.Web.Profile.Models
{
    public class WaitlistViewModel : GmWeb.Web.Common.Models.BasePageViewModel
    {
        public Flow Flow { get; set; }
    }
}