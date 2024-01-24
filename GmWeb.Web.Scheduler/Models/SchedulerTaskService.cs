using System.IO;
using Ical.Net.CalendarComponents;

namespace GmWeb.Web.Scheduler.Models
{
    using System.Linq;
    using Kendo.Mvc.UI;
    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using GmWeb.Web.Scheduler.Extensions;
    using GmWeb.Web.Scheduler.Converter;
    using Kendo.Mvc.Extensions;
    using Ical.Net.Serialization;
    using System.Diagnostics;
    using System.Text;
    using Ical.Net.DataTypes;
    using GmWeb.Logic.Data.Context.Carma;
    using GmWeb.Logic.Data.Models.Carma;
    using System.Security.Claims;

    public class SchedulerTaskService : BaseService, ISchedulerEventService<TaskViewModel>
    {
        private readonly CarmaContext _context;
        private ISession _session;
        private readonly ClaimsPrincipal _user;
        static private bool bImportFromClientData = false;
        static private bool bImportNewEvents = false;
        public static string strColor = "#f8a398";
        public ISession Session { get { return _session; } }

        public SchedulerTaskService(IHttpContextAccessor httpContextAccessor, CarmaContext context)
        {
            _user = httpContextAccessor.HttpContext.User;
            _session = httpContextAccessor.HttpContext.Session;
            _context = context;
        }

        public override IList<TaskViewModel> ReadImportedEvents()
        {
            Ical.Net.Calendar iCalendar = Ical.Net.Calendar.Load(m_strEventsFromClient);
            var alreadyExistsEvents = Session.GetObjectFromJson<IList<TaskViewModel>>("SchedulerTasks");

            IList<Attendee> importedAtendees = new List<Attendee>();

            IList<TaskViewModel> listofImportedEvents = KendoSchedulerICalendarImporter.ImportEvents(iCalendar.Events, ref importedAtendees);
            foreach (TaskViewModel existEvent in alreadyExistsEvents)
            {
                var dublicateEvent = listofImportedEvents.FirstOrDefault(e => e.UID == existEvent.UID); //mail has ot be unique
                if (dublicateEvent != null)
                    listofImportedEvents.Remove(dublicateEvent);
            }
            return listofImportedEvents;
        }
        public override void InsertAttendee(Attendee attendee)
        {
            var participants =  GetAttendees();
            var first = participants.OrderByDescending(e => e.Value).FirstOrDefault();

            var id = (first != null) ? first.Value : 0;

            attendee.Value = id + 1;
            attendee.Color = strColor;
            attendee.Text = attendee.Name ?? attendee.Mail;

            participants.Insert(0, attendee);

            Session.SetObjectAsJson("Attendees", participants);
        }

        public override void UpdateAttendee(Attendee attendee, ModelStateDictionary modelState)
        {
               var atendees = GetAttendees();
               var target = atendees.FirstOrDefault(e => e.Value == attendee.Value);
               if (target != null)
               {
                    target.Mail = attendee.Mail;
                    target.Name = attendee.Name;
                    target.Text = attendee.Name ?? attendee.Mail;
                }
               Session.SetObjectAsJson("Attendees", atendees);
        }
        public override void DestroyAttendee(Attendee attendee, ModelStateDictionary modelState)
        {
                try
                {
                    var atendees = GetAttendees();
                    var target = atendees.FirstOrDefault(e => e.Value == attendee.Value);

                    if (target != null)
                    {
                        var events = GetAllTasks();
                        foreach (TaskViewModel item in events)
                        {
                            var atendeeRemoved = item.Attendees.FirstOrDefault(e => e == target.Value);
                            if (atendeeRemoved != default)
                                item.Attendees.Remove(atendeeRemoved);
                        }
                        atendees.Remove(target);
                    }

                    Session.SetObjectAsJson("Attendees", atendees);
                }
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                }
        }
        public override IList<Attendee> GetAttendees()
        {
            var result = Session.GetObjectFromJson<IList<Attendee>>("Attendees");

            if (result == null)
            {
                result = new List<Attendee>();
                Session.SetObjectAsJson("Attendees", result);
            }
            return result;
        }

        public virtual IQueryable<TaskViewModel> GetAll()
        {
            return GetAllTasks().AsQueryable();
        }

        internal static string ReadStream(string strFilePath)
        {
            try
            {
                StreamReader reader = new(strFilePath);
                string strFileContent = reader.ReadToEnd();
                reader.Close();
                return strFileContent;
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return "";
            }
        }

        internal static void WriteStream(string strFilePath, string strBuffer)
        {
            try
            {
                StreamWriter writer = new(strFilePath);
                writer.Write(strBuffer);
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
            
        }

        static private string m_strEventsFromClient = "";
        static private string m_strDataFromClient = "";
        static private string m_strDataFileNameFromClient = "";


        public override void ImportFilteredEvents(IEnumerable<string> arrayOfValues)
        {
            if (arrayOfValues.Count() == 0) //nothing to import,  switch flag to avoid re-init
            {
                m_strEventsFromClient = "";
                bImportNewEvents = false;
                return;
            }
            Ical.Net.Calendar iCalendar = Ical.Net.Calendar.Load(m_strEventsFromClient);

            foreach (CalendarEvent calEvent in new List<CalendarEvent>(iCalendar.Events) )
            {
                var founded = arrayOfValues.FirstOrDefault(e => e == calEvent.Uid);
                if (founded == null)
                    iCalendar.Events.Remove(calEvent);
            }
            var serializer = new CalendarSerializer();
            m_strEventsFromClient = serializer.SerializeToString(iCalendar);
            bImportNewEvents = true;
        }

        public override void ImportEvents(string strEvents, string strIcalFilePath)
        {
            m_strEventsFromClient = strEvents;
            bImportNewEvents = true;
        }
        public override void ImportICalData(string strIcalData, string strIcalName)
        {
            SaveChanges();
            m_strDataFileNameFromClient = strIcalName;
            m_strDataFromClient = strIcalData;
            bImportFromClientData = true;
        }
        public override string ExportICalData()
        {
            try
            {
                return SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return "";
            }
        }

        public override string SaveChanges()
        {
            try
            {
                //imitate db read
                Ical.Net.Calendar iCalendar = null;

                var firstCalendar = _context.ActivityCalendars.FirstOrDefault();
                if (firstCalendar != null && !bImportFromClientData)
                {
                    iCalendar = Ical.Net.Calendar.Load(Encoding.Default.GetString(firstCalendar.FileStream) );
                }
                else if (bImportFromClientData)
                    iCalendar = Ical.Net.Calendar.Load(m_strDataFromClient);
                if (iCalendar == null)
                    iCalendar = Ical.Net.Calendar.Load(GmWeb.Web.Scheduler.Properties.Resources.iCalTestData); //template for filling

                //Get data from Session
                var result = Session.GetObjectFromJson<IList<TaskViewModel>>("SchedulerTasks");
                var kendoAtendees = GetAttendees();
                string serialized = "";
                if (result != null)
                {
                    //remove unnecessary events from iCal collection
                    var itemsToDelete =  new List <Ical.Net.CalendarComponents.CalendarEvent> (iCalendar.Events.Where(p => result.All(p2 => p2.UID != p.Uid)));
                    foreach (var ce in itemsToDelete)
                        iCalendar.Events.Remove(ce);

                    foreach (TaskViewModel kendoItem in result)
                    {
                        if (kendoItem.Modified)
                        {
                            if (kendoItem.IsEvent)
                            {
                                bool bNewEvent = false;
                                var iCalItem = iCalendar.Events.FirstOrDefault(e => e.Uid == kendoItem.UID);
                                if (iCalItem == null)
                                {
                                    bNewEvent = true;
                                    iCalItem = new CalendarEvent();
                                    iCalItem.Uid = kendoItem.UID;
                                    iCalItem.Attendees = new List<Ical.Net.DataTypes.Attendee>();
                                    iCalItem.RecurrenceRules = new List<Ical.Net.DataTypes.RecurrencePattern>();
                                    iCalItem.ExceptionRules = new List<Ical.Net.DataTypes.RecurrencePattern>();
                                    iCalItem.DtStart = new Ical.Net.DataTypes.CalDateTime(kendoItem.Start); 
                                    iCalItem.DtEnd =  new Ical.Net.DataTypes.CalDateTime(kendoItem.End);
                                    iCalendar.Events.Add(iCalItem);
                                }
                                    
                                iCalItem.Summary = kendoItem.Title;
                                
                                if (!bNewEvent)
                                {
                                    iCalItem.DtStart/*.Value*/ = new Ical.Net.DataTypes.CalDateTime(kendoItem.Start);
                                    iCalItem.DtEnd/*.Value*/ = new Ical.Net.DataTypes.CalDateTime(kendoItem.End);
                                }

                                iCalItem.IsAllDay = kendoItem.IsAllDay;
                                iCalItem.Description = kendoItem.Description;
                                    
                                if (!kendoItem.IsAllDay)
                                {
                                    if (kendoItem.StartTimezone != null)
                                    {
                                        iCalItem.DtStart.TzId = kendoItem.StartTimezone;
                                    }
                                    if (kendoItem.EndTimezone != null)
                                    {
                                        iCalItem.DtEnd.TzId = kendoItem.EndTimezone;
                                    }
                                }

                                if (kendoItem.RecurrenceException != null)
                                {
                                    iCalItem.ExceptionRules = new List<RecurrencePattern>();
                                    RecurrencePattern r_pattern = new RecurrencePattern(kendoItem.RecurrenceException);
                                    iCalItem.ExceptionRules.Add(r_pattern);

                                }
                                if (kendoItem.RecurrenceRule != null)
                                {
                                    iCalItem.RecurrenceRules = new List<RecurrencePattern>();
                                    RecurrencePattern r_pattern = new RecurrencePattern(kendoItem.RecurrenceRule);
                                    iCalItem.RecurrenceRules.Add(r_pattern);
                                }
                                if (kendoItem.Attendees.Any())
                                {
                                    iCalItem.Attendees.Clear();
                                    foreach (int kendoAtendeeID in kendoItem.Attendees)
                                    {
                                        Attendee kendoAtendee = kendoAtendees.FirstOrDefault(e=> e.Value == kendoAtendeeID);
                                        Ical.Net.DataTypes.Attendee iCalendarAtendee = new Ical.Net.DataTypes.Attendee();
                                        iCalendarAtendee.CommonName = kendoAtendee.Name;
                                        iCalendarAtendee.Value = new System.Uri(kendoAtendee.Mail);
                                        iCalItem.Attendees.Add(iCalendarAtendee);
                                    }
                                }
                                else
                                    iCalItem.Attendees.Clear();
                            }
                            else if (kendoItem.IsTodo)
                            {

                            }
                            else if (kendoItem.IsJournal)
                            {

                            }
                        }
                    }

                    var serializer = new CalendarSerializer();
                    bool bNew = false;
                    var usCal = _context.ActivityCalendars.FirstOrDefault(c => c.Name == m_strDataFileNameFromClient);
                    if (usCal == null)
                    {
                        bNew = true;
                        usCal = new ActivityCalendar();
                        usCal.CalendarID = Guid.NewGuid();
                        usCal.Name = m_strDataFileNameFromClient; 
                        usCal.IsDirectory = false;
                        usCal.IsOffline = false;
                        usCal.IsHidden = false;
                        usCal.IsReadOnly = false;
                        usCal.IsArchive = true;
                        usCal.IsSystem = false;
                        usCal.IsTemporary = false;
                        usCal.CreationTime = DateTimeOffset.Now;
                    }
                    serialized = serializer.SerializeToString(iCalendar);
                    usCal.FileStream = Encoding.Default.GetBytes(serialized);
                    usCal.LastWriteTime = DateTimeOffset.Now;
                    usCal.FileType = "ics";

                    if (bNew)
                        _context.ActivityCalendars.Add(usCal);
                    else
                    {
                        _context.ActivityCalendars.Update(usCal);
                    }
                                
                    _context.SaveChanges();
                    Session.SetObjectAsJson("SchedulerTasks", result);
                }
                return serialized;
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return "";
            }
        }
        private IList<TaskViewModel> GetAllTasks()
        {
            try
            {

                var curUserMail = _user.FindFirstValue(ClaimTypes.Email);

                var usCal = _context.ActivityCalendars.FirstOrDefault(); //TODO load the necessary  row from DB - for current logged user
                var result = Session.GetObjectFromJson<IList<TaskViewModel>>("SchedulerTasks");

                if (result == null || bImportFromClientData || bImportNewEvents)
                {

                    if (!bImportNewEvents)
                        result = new List<TaskViewModel>();

                    Ical.Net.Calendar iCalendar = null;

                    if (bImportNewEvents && m_strEventsFromClient.Length > 5 && result != null)
                    {
                        iCalendar = Ical.Net.Calendar.Load(m_strEventsFromClient);

                        var alreadyExistsAtendees = GetAttendees();
                        IList<Attendee> importedAtendees = new List<Attendee>();

                        IList<TaskViewModel> listofImportedEvents = KendoSchedulerICalendarImporter.ImportEvents(iCalendar.Events, ref importedAtendees);
                        foreach (Attendee attendee in importedAtendees)
                        {
                            var alreadyExist = alreadyExistsAtendees.FirstOrDefault(e => e.Mail == attendee.Mail); //mail has ot be unique
                            if (alreadyExist == null)
                                alreadyExistsAtendees.Add(attendee);
                        }
                        Session.SetObjectAsJson("Attendees", alreadyExistsAtendees); //save parsed atendees
                        foreach (TaskViewModel newEvent in listofImportedEvents)
                        {
                            var alreadyExist = result.FirstOrDefault(e => e.UID == newEvent.UID); //mail has ot be unique
                            if (alreadyExist == null)
                                result.Add(newEvent);
                        }
                        Session.SetObjectAsJson("SchedulerTasks", result);
                        bImportNewEvents = false;
                        return result;
                    }

                    if (bImportFromClientData)
                    {
                        iCalendar = Ical.Net.Calendar.Load(m_strDataFromClient);
                    }
                    else if (usCal != null)
                    {
                        iCalendar = Ical.Net.Calendar.Load(Encoding.Default.GetString(usCal.FileStream));
                        m_strDataFileNameFromClient = usCal.Name;
                    }

                    if (iCalendar == null)
                        return result;


                    var kendoAtendees = GetAttendees();
                    IList<TaskViewModel> listEvents = KendoSchedulerICalendarImporter.ImportEvents(iCalendar.Events, ref kendoAtendees);
                    Session.SetObjectAsJson("Attendees", kendoAtendees); //save parsed atendees

                    foreach (TaskViewModel newEvent in listEvents)
                    {
                        var alreadyExist = result.FirstOrDefault(e => e.UID == newEvent.UID);
                        if (alreadyExist == null)
                            result.Add(newEvent);
                    }
                }
                Session.SetObjectAsJson("SchedulerTasks", result);

                bImportFromClientData = false;

                return result;

            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return new List<TaskViewModel>();
            }
        }


        public virtual void Insert(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {
                    var tasks = GetAllTasks();
                    var first = tasks.OrderByDescending(e => e.ID).FirstOrDefault();

                    var targetExist = tasks.FirstOrDefault(e => e.ID == task.ID);
                    if (targetExist != null)
                        return;

                    var id = (first != null) ? first.ID : 0;

                    task.ID = id + 1;
                    task.Modified = true;
                    task.IsEvent = true;
                    task.UID = Guid.NewGuid().ToString();
                    tasks.Insert(0, task);

                    Session.SetObjectAsJson("SchedulerTasks", tasks);
            }
        }

        public virtual void Update(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {
                    var tasks = GetAllTasks();
                    var target = tasks.FirstOrDefault(e => e.ID == task.ID);
                    if (target != null)
                    {
                        target.Title = task.Title;
                        target.Start = task.Start;
                        target.End = task.End;
                        target.StartTimezone = task.StartTimezone;
                        target.EndTimezone = task.EndTimezone;
                        target.Description = task.Description;
                        target.IsAllDay = task.IsAllDay;
                        target.RecurrenceRule = task.RecurrenceRule;
                        target.RecurrenceException = task.RecurrenceException;
                        target.RecurrenceID = task.RecurrenceID;
                        target.OwnerID = task.OwnerID;
                        target.Attendees = task.Attendees;
                        target.Modified = true;
                    }

                    Session.SetObjectAsJson("SchedulerTasks", tasks);
            }
        }

        public virtual void Delete(TaskViewModel task, ModelStateDictionary modelState)
        {

                var tasks = GetAllTasks();
                var target = tasks.FirstOrDefault(e => e.ID == task.ID);

                if (target != null)
                {
                    tasks.Remove(target);

                    var recurrenceExceptions = tasks.Where(m => m.RecurrenceID == task.ID).ToList();

                    foreach (var recurrenceException in recurrenceExceptions)
                    {
                        tasks.Remove(recurrenceException);
                    }
                }

                Session.SetObjectAsJson("SchedulerTasks", tasks);
        }

        //TODO: better naming or refactor
        private bool ValidateModel(TaskViewModel appointment, ModelStateDictionary modelState)
        {
            if (appointment.Start > appointment.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }
            
            return true;
        }
    }
}