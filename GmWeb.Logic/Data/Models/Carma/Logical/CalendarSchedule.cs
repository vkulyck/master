using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using iCalEvent = Ical.Net.CalendarComponents.CalendarEvent;
using iCalPeriod = Ical.Net.DataTypes.Period;
using iCalRecurrencePattern = Ical.Net.DataTypes.RecurrencePattern;

namespace GmWeb.Logic.Data.Models.Carma
{
    public class CalendarSchedule : CalendarItem
    {
        public List<CalendarRecurrence> RecurrenceRules { get; set; } = new List<CalendarRecurrence>();

        public override void Load(CarmaCache cache, AgencyCalendarFile calFile, iCalEvent calEvent)
        {
            base.Load(cache, calFile, calEvent);
            var casts = calEvent.RecurrenceRules.Select(x => (CalendarRecurrence)x).ToList();
            this.RecurrenceRules.AddRange(casts);
        }

        public override void Store(CarmaCache cache, iCalEvent calEvent)
        {
            base.Store(cache, calEvent);
            var casts = this.RecurrenceRules.Select(x => (iCalRecurrencePattern)x).ToList();
            calEvent.RecurrenceRules.AddRange(casts);
        }
    }
}
