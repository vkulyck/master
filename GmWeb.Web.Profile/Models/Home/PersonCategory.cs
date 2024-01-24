using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Profile.Models.Home
{
    public class PersonCategory : IDataRowModel
    {
        public string BriefDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ServiceTypeDescription { get; set; }
        public int MaxAttendees { get; set; }
        public double FeeToClient { get; set; }
        public string VenueAddressNo { get; set; }
        public string VenueAddressStreet { get; set; }
        public string VenueAddressStreetType { get; set; }
        public string VenueZip { get; set; }

        public bool IsActionNeeded { get; set; }

        public void CopyRowData(DataRow row) => this.PerformDefaultRowConversion(row);
    }
}