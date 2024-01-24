using System;

namespace GmWeb.Logic.Data.Models.Carma
{
    public class Activity : CalendarOccurrence
    {
        public Guid ActivityID
        {
            get => this.OccurrenceID;
            set => this.OccurrenceID = value;
        }
    }
}
