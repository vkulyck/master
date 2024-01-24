using GmWeb.Logic.Data.Context.Carma;
using System;
using iCalEvent = Ical.Net.CalendarComponents.CalendarEvent;
using iCalPeriod = Ical.Net.DataTypes.Period;

namespace GmWeb.Logic.Data.Models.Carma
{
    public abstract class CalendarOccurrence : CalendarItem
    {
        public Guid OccurrenceID { get; set; }
        public virtual void Load(CarmaCache cache, iCalPeriod period, AgencyCalendarFile calFile, iCalEvent calEvent)
        {
            base.Load(cache, calFile, calEvent);
            this.StartTime = period.StartTime.AsSystemLocal;
            this.EndTime = this.StartTime + calEvent.Duration;
            this.OccurrenceID = this.GenerateOccurrenceId(calEvent);
        }

        public Guid GenerateOccurrenceId(iCalEvent calEvent)
        {
            var dg = new ActivityGuid(calEvent.Uid, this.StartTime);
            return dg.ActivityID;
        }
    }
}
