using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Web.Common.Models.Carma;


namespace GmWeb.Web.Api.Models.Common
{
    #region Calendar Items
    public class CalendarItemDTO
    {
        public Guid CalendarID { get; set; }
        public Guid EventID { get; set; }
        public int AgencyID { get; set; }
        public int? OrganizerID { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    
    public class CalendarDetailsDTO : CalendarItemDTO
    {
        public UserDTO Organizer { get; set; }
        public string Location { get; set; }
        public int? Capacity { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string EventType { get; set; }
        public decimal? Contribution { get; set; }
    }
    #endregion
    #region Schedules
    public class ScheduleUpsertDTO
    {
        public List<CalendarRecurrence> RecurrenceRules { get; set; }
        public int? OrganizerID { get; set; }
        public int? Capacity { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string EventType { get; set; }
        public decimal? Contribution { get; set; }
    }
    public class ScheduleInsertDTO : ScheduleUpsertDTO
    {
        [Required]
        public int AgencyID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Location { get; set; }
    }
    
    public class ScheduleUpdateDTO : ScheduleUpsertDTO
    {
        [Required]
        public Guid EventID { get; set; }
        public int? AgencyID { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Location { get; set; }
    }
    #endregion

    #region Activities
    public class ActivityDTO : CalendarItemDTO
    {
        public Guid ActivityID { get; set; }
    }
    public class ActivityDetailsDTO : CalendarDetailsDTO
    {
        public Guid ActivityID { get; set; }
    }
    #endregion
}
