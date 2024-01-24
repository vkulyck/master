using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class Client
    {
        public int? ClientID { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public int? FamilySize { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
    }
}
