using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Extensions.Calendar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using iCalDateTime = Ical.Net.DataTypes.CalDateTime;
using iCalEvent = Ical.Net.CalendarComponents.CalendarEvent;
using iCalOrganizer = Ical.Net.DataTypes.Organizer;
using iCalPeriod = Ical.Net.DataTypes.Period;

namespace GmWeb.Logic.Data.Models.Carma
{
    public abstract class CalendarItem
    {
        [NotMapped]
        public virtual IList<User> InvitedGuests { get; } = new List<User>();
        [NotMapped]
        public int? OrganizerID => this.Organizer?.UserID;
        [NotMapped]
        public virtual User Organizer { get; set; }
        public Guid CalendarID { get; set; }
        public Guid EventID { get; set; }
        public string Name { get; set; }
        public int AgencyID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        [CalendarProperty]
        public int? Capacity { get; set; }
        [CalendarProperty]
        public string EventType { get; set; }
        [CalendarProperty]
        public decimal? Contribution { get; set; }

        protected User EmailToClient(CarmaCache cache, Uri emailUri)
        {
            string email = emailUri.AbsoluteUri.ToLower().Replace("mailto:", "");
            // NOTE: Replaced UserIdentities lookup with Clients.Email column to 
            // support Client instances not affiliated with Identity accounts.
            // UserIdentities have not been removed entirely to help maintain
            // cross-catalog query compatibility, Identity-derived default values,
            // etc.
            return cache.Users.SingleOrDefault(x => x.Email.ToLower() == email);
        }

        public virtual void Load(CarmaCache cache, AgencyCalendarFile calFile, iCalEvent calEvent)
        {
            this.LoadProperties(calEvent);
            this.CalendarID = calFile.CalendarID;
            this.EventID = new Guid(calEvent.Uid);
            this.Name = calEvent.Summary;
            this.AgencyID = calFile.AgencyID;
            this.Location = calEvent.Location;
            if(calEvent.Organizer != null)
                this.Organizer = this.EmailToClient(cache, calEvent.Organizer.Value);
            this.Description = calEvent.Description;
            this.StartTime = calEvent.DtStart.Value;
            this.EndTime = calEvent.DtEnd.Value;

            foreach (var attendee in calEvent.Attendees)
            {
                var attClient = this.EmailToClient(cache, attendee.Value);
                if (attClient == null)
                    continue;
                this.InvitedGuests.Add(attClient);
            }
        }
        public virtual void Store(CarmaCache cache, iCalEvent calEvent)
        {
            this.StoreProperties(calEvent);
            calEvent.Uid = this.EventID.ToString();
            calEvent.Summary = this.Name;
            calEvent.DtStart = new iCalDateTime(this.StartTime);
            calEvent.DtEnd = new iCalDateTime(this.EndTime);
            calEvent.Location = this.Location;
            if (!string.IsNullOrWhiteSpace(this.Description))
                calEvent.Description = this.Description;
            if (this.Organizer != null)
            {
                calEvent.Organizer = new iCalOrganizer
                {
                    CommonName = $"{this.Organizer.LastName}, {this.Organizer.FirstName}",
                    Value = new Uri($"mailto:{this.Organizer.Email}")
                };
            }
        }
    }
}
