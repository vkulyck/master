namespace GmWeb.Web.Scheduler.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kendo.Mvc.UI;

    public class TaskViewModel : ISchedulerEvent
    {
        private IList<int> m_attendees = new List<int>();
        public IList<int> Attendees
        {
            get
            {
                return m_attendees;
            }
            set
            {
                m_attendees = value;
            }
        }

        private bool m_bModified = false;
        public bool Modified
        {
            get
            {
                return m_bModified;
            }
            set
            {
                m_bModified = value;
            }
        }

        private bool m_bEvent = false;
        public bool IsEvent
        {
            get
            {
                return m_bEvent;
            }
            set
            {
                m_bEvent = value;
            }
        }
        private bool m_bToDo = false;
        public bool IsTodo
        {
            get
            {
                return m_bToDo;
            }
            set
            {
                m_bToDo = value;
            }
        }
        private bool m_bJournal = false;
        public bool IsJournal
        {
            get
            {
                return m_bJournal;
            }
            set
            {
                m_bJournal = value;
            }
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        private DateTime start;
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        public string StartTimezone { get; set; }

        private DateTime end;
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public int? OwnerID { get; set; }

        public int? RoomID { get; set; }

        public string UID { get; set; }
    }
}