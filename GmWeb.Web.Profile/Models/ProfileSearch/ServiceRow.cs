using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public class ServiceRow : IDataRowModel
    {
        public bool IsChecked { get; set; }
        public int CategoryID { get; set; }

        public string Description { get; set; }
        public string BriefDescription { get; set; }
        public string AgencyName { get; set; }
        public string Venue { get; set; }
        public string VenueAddressNo { get; set; }
        public string VenueAddressStreet { get; set; }
        public string VenueAddressStreetType { get; set; }
	    public string City { get; set; }
	    public string State { get; set; }
        public string VenueZip { get; set; }
        public void CopyRowData(DataRow row)
        {
            this.PerformDefaultRowConversion(row);
            this.IsChecked = false;
        }
    }
}