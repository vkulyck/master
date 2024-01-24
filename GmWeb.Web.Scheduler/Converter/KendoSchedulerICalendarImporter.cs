using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GmWeb.Web.Scheduler.Models;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Proxies;

namespace GmWeb.Web.Scheduler.Converter
{
    public class KendoSchedulerICalendarImporter
    {



        /// Imports Events, read from the iCalendar file, in KendoScheduler.
        private static int iID = 1;
        public static IList<TaskViewModel> ImportEvents(IUniqueComponentList<CalendarEvent> calEvents, ref IList<Attendee> KendoAtendees)
        {
            IList<TaskViewModel> schedulerTasks = new List<TaskViewModel>();
            foreach (CalendarEvent calEvent in calEvents)
            {
                TaskViewModel calendarAct = new TaskViewModel();
                calendarAct.Attendees = new List<int>();
                calendarAct.UID = calEvent.Uid;
                calendarAct.IsEvent = true;
                calendarAct.ID = iID++;
                calendarAct.Title = calEvent.Summary ?? String.Empty;
                calendarAct.Start = calEvent.DtStart.AsUtc;
                calendarAct.End = calEvent.DtEnd != null ? calEvent.DtEnd.AsUtc : calEvent.DtStart.AsUtc;
                

                calendarAct.Description = calEvent.Description ?? String.Empty;
                calendarAct.IsAllDay = calEvent.IsAllDay;
                if (calEvent.DtEnd != null && calEvent.DtEnd.Value > calEvent.DtStart.Value)
                    calendarAct.IsAllDay = false;

                if (!calendarAct.IsAllDay)
                {
                    if (!calEvent.DtStart.IsUtc && !calEvent.DtEnd.IsUtc)
                    {
                        try
                        {
                            calendarAct.StartTimezone = calEvent.DtStart.TzId;
                            calendarAct.EndTimezone = calEvent.DtEnd.TzId;
                        }
                        catch (Exception ex)
                        {
                            Debug.Assert(false, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                            calendarAct.StartTimezone = calEvent.DtStart.TzId;
                            calendarAct.EndTimezone = calEvent.DtEnd.TzId;
                        }
                    }
                }

                if (calEvent.RecurrenceRules.Any())
                    calendarAct.RecurrenceRule = calEvent.RecurrenceRules.FirstOrDefault().ToString();

                if (calEvent.ExceptionRules.Any())
                    calendarAct.RecurrenceException = calEvent.ExceptionRules.FirstOrDefault().ToString();

                if (calEvent.Attendees.Any())
                {

                    foreach (Ical.Net.DataTypes.Attendee attendee in calEvent.Attendees)
                    {
                        //getting ID for searching / inserting
                        var first = KendoAtendees.OrderByDescending(e => e.Value).FirstOrDefault();
                        var id = (first != null) ? first.Value : 0;
                        id++;

                        var alreadyExist = KendoAtendees.FirstOrDefault(e => e.Mail == attendee.Value.ToString()); //mail has ot be unique
                        if (alreadyExist == null)
                        {
                            //add to general collection
                            Attendee atn = new()
                            {
                                Text =  attendee.CommonName??  attendee.Value.ToString(),
                                Name = attendee.CommonName??  attendee.Value.ToString(),
                                Value = id,
                                Color = SchedulerTaskService.strColor,
                                Mail = attendee.Value.ToString(),
                            };
                            KendoAtendees.Add(atn);
                            //add to certain event
                            calendarAct.Attendees.Add(atn.Value);
                        }
                        else
                        {
                            calendarAct.Attendees.Add(alreadyExist.Value);
                        }
                    }
                }
                    


                schedulerTasks.Add(calendarAct);
            }
            return schedulerTasks;
        }

        /// Imports the todos.
        public static IList<TaskViewModel> ImportTodos(IUniqueComponentList<Todo> calToDo)
        {
            IList<TaskViewModel> schedulerToDo = new List<TaskViewModel>();
            foreach (Todo todo in calToDo)
            {
                if (todo.Start != null || todo.DtStamp != null)
                {
                    TaskViewModel calendarToDo = new TaskViewModel();
                    calendarToDo.UID = todo.Uid;
                    calendarToDo.IsTodo = true;
                    calendarToDo.ID = iID++;
                    calendarToDo.Title = todo.Summary ?? String.Empty;
                    calendarToDo.Start = todo.Start  != null ? todo.Start.Value : todo.DtStamp.Value;
                    //Import To-Dos as All Day events as they do not have End Date.
                    calendarToDo.End = calendarToDo.Start.AddDays(1);
                    if (todo.RecurrenceRules.Any())
                        calendarToDo.RecurrenceRule = todo.RecurrenceRules.FirstOrDefault().ToString();

                    if (todo.Attendees.Any())
                    {
                        foreach (Ical.Net.DataTypes.Attendee attendee in todo.Attendees)
                        {
                            //calendarToDo.Attendees.Add(attendee.Value.ToString());
                        }
                    }

                        //calendarToDo.Attendee = todo.Attendees.FirstOrDefault().Value.ToString();

                    if (todo.ExceptionRules.Any())
                        calendarToDo.RecurrenceException = todo.ExceptionRules.FirstOrDefault().ToString();

                    calendarToDo.IsAllDay = true;
                    calendarToDo.Description = todo.Description ?? String.Empty;
                    
                    
                    schedulerToDo.Add(calendarToDo);
                }
            }
            return schedulerToDo;
        }

        /// Imports Journals, read from the iCalendar file, as All Day events in KendoScheduler.
        public static IList<TaskViewModel> ImportJournals(ICalendarObjectList<Journal> journals)
        {
            IList<TaskViewModel> schedulerJournal = new List<TaskViewModel>();
            foreach (Journal journal in journals)
            {
                if (journal.Start != null || journal.DtStamp != null)
                {
                    TaskViewModel calendarJournal = new TaskViewModel();

                    calendarJournal.ID = iID++;
                    calendarJournal.IsJournal = true;
                    calendarJournal.UID = journal.Uid;

                    calendarJournal.Title = journal.Summary ?? String.Empty;
                    calendarJournal.Start = journal.Start != null ? journal.Start.Value : journal.DtStamp.Value;
                    
                    //Import To-Dos as All Day events as they do not have End Date.
                    calendarJournal.End = calendarJournal.Start.AddDays(1);
                    if (journal.RecurrenceRules.Any())
                        calendarJournal.RecurrenceRule = journal.RecurrenceRules.FirstOrDefault().ToString();

                    if (journal.Attendees.Any())
                    {
                          foreach (Ical.Net.DataTypes.Attendee attendee in journal.Attendees)
                          {
                            //calendarJournal.Attendees.Add(attendee.Value.ToString());
                          }
                    }

                    //calendarJournal.Attendee = journal.Attendees.FirstOrDefault().Value.ToString();

                    if (journal.ExceptionRules.Any())
                        calendarJournal.RecurrenceException = journal.ExceptionRules.FirstOrDefault().ToString();

                    calendarJournal.IsAllDay = true;
                    calendarJournal.Description = journal.Description ?? String.Empty;
                    

                    schedulerJournal.Add(calendarJournal);
                }
            }
            return schedulerJournal;
        }

    }
}
