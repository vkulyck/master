using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public class ServiceDetailViewModel : GmWeb.Logic.Interfaces.IDataRowModel
    {
        public string SelectedActivity { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BriefDescription { get; set; }
        public string Venue { get; set; }
        public string VenueAddressNo { get; set; }
        public string VenueAddressStreet { get; set; }
        public string VenueCity { get; set; }
        public string VenueSuite { get; set; }
        public string FeeToClient { get; set; }
        public string MaxAttendees { get; set; }
        public string CurrentAttendees { get; set; }
        public string RemainingSlots { get; set; }
        public string ServiceType { get; set; }
        public string Neighborhood { get; set; }
        public string VenueAddressStreetType { get; set; }

        public void CopyRowData(DataRow row)
        {
            this.SelectedActivity = row.ToString("ActivityDescription");
            this.Description = row.ToString("Description");
            this.StartDate = row.ToDateTime("StartDate").ToString("MMM d, yyyy");
            this.EndDate = row.ToDateTime("EndDate").ToString("MMM d, yyyy");
            this.BriefDescription = row.ToString("BriefDescription");

            this.Venue = row.ToString("Venue");
            this.VenueAddressNo = row.ToString("VenueAddressNo");
            this.VenueAddressStreet = row.ToString("VenueAddressStreet");
            this.VenueCity = string.Format("{0}, {1} {2}", row.ToString("City"), row.ToString("State"), row.ToString("VenueZip"));
            this.VenueSuite = row.ToString("VenueSuite");

            this.FeeToClient = row.ToDecimal("FeeToClient").ToString("c");
            this.MaxAttendees = row.ToString("MaxAttendees");
            string attendees = row.ToString("CurrentAttendees"), slots = row.ToString("RemainingSlots");
            this.CurrentAttendees = string.IsNullOrEmpty(attendees) ? "0" : attendees;
            this.RemainingSlots = string.IsNullOrEmpty(slots) ? "0" : slots;

            this.ServiceType = row.ToString("ServiceTypeDescription");
            this.Neighborhood = row.ToString("NeighborhoodDescr");
            this.VenueAddressStreetType = row.ToString("VenueAddressStreetType");
        }
    }
}