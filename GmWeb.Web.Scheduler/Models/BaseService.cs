
using GmWeb.Logic.Data.Context;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace GmWeb.Web.Scheduler.Models
{
    public abstract class BaseService
    {
        public virtual string SaveChanges()
        {
            return "";
        }
        public virtual void ImportFilteredEvents(IEnumerable<string> arrayOfValues)
        {

        }
        public virtual void ImportEvents(string strEvents, string strIcalFilePath)
        {

        }
        public virtual void ImportICalData(string strIcalData, string strIcalName)
        {

        }
        public virtual string ExportICalData()
        {
            return "";
        }

        public virtual IList<TaskViewModel> ReadImportedEvents()
        {
            return null;
        }
        public virtual IList<Attendee> GetAttendees()
        {
            return null;
        }
        public virtual void InsertAttendee(Attendee attendee)
        {
            
        }
        public virtual void UpdateAttendee(Attendee attendee, ModelStateDictionary modelState)
        {
            
        }
        public virtual void DestroyAttendee(Attendee attendee, ModelStateDictionary modelState)
        {
            
        }
    }
}
