using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using iCalEvent = Ical.Net.CalendarComponents.CalendarEvent;
using iRecurUtil = Ical.Net.Evaluation.RecurringEvaluator;

namespace GmWeb.Logic.Data.Context.Carma
{
    public class CalendarFiles
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public CarmaCache Cache { get; private set; }
        public CalendarFiles(CarmaCache cache)
        {
            this.Cache = cache;
        }

        public CalendarSchedule GetSchedule(AgencyCalendarFile calFile, Guid scheduleID)
        {
            var cal = calFile.Deserialize();
            foreach (var calEvent in cal.Events)
            {
                if (calEvent.Uid != scheduleID.ToString())
                    continue;

                var schedule = new CalendarSchedule();
                schedule.Load(this.Cache, calFile, calEvent);
                return schedule;
            }
            return null;
        }
        public IEnumerable<TOccurrence> GetOccurrences<TOccurrence>(AgencyCalendarFile calFile, Guid occurrenceID)
            where TOccurrence : CalendarOccurrence, new()
        {
            var ag = new ActivityGuid(occurrenceID);
            var cal = calFile.Deserialize();
            foreach (var calEvent in cal.Events)
            {
                var hg = new HalfGuid(calEvent.Uid);
                if (hg != ag.EventID)
                    continue;
                var occurrences = this.GetOccurrences<TOccurrence>(calFile, calEvent, ag.FilterRange, occurrenceID);
                foreach (var occurrence in occurrences)
                    yield return occurrence;
            }
        }
        public IEnumerable<TOccurrence> GetOccurrences<TOccurrence>(AgencyCalendarFile calFile, DateRange range, Guid? eventID = null, Guid? occurrenceID = null)
            where TOccurrence : CalendarOccurrence, new()
        {
            var cal = calFile.Deserialize();
            foreach (var calEvent in cal.Events)
            {
                if (eventID.HasValue && calEvent.Uid != eventID.ToString())
                    continue;
                var occurrences = this.GetOccurrences<TOccurrence>(calFile, calEvent, range, occurrenceID);
                foreach (var occurrence in occurrences)
                    yield return occurrence;
            }
        }

        protected IEnumerable<TOccurrence> GetOccurrences<TOccurrence>(AgencyCalendarFile calFile, iCalEvent calEvent, DateRange range, Guid? occurrenceID)
            where TOccurrence : CalendarOccurrence, new()
        {
            var eval = new iRecurUtil(calEvent);
            var refDate = calEvent.DtStart;
            bool includeRefDate = range.Contains(refDate.AsSystemLocal);
            var periods = eval.Evaluate(refDate, range.Start, range.End, includeRefDate)
                // eval.Evaluate includes periods from the 24 hours
                // preceding the range start, so we have to filter
                // out periods that fall outside the range.
                .Where(x => range.Contains(x.StartTime.Value))
                .ToList()
            ;
            foreach (var period in periods)
            {
                var occurrence = new TOccurrence();
                occurrence.Load(this.Cache, period, calFile, calEvent);
                if (occurrenceID.HasValue && occurrence.OccurrenceID != occurrenceID)
                    continue;
                yield return occurrence;
            }
        }
    }
}
