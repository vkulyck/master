using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Carma
{
    [Table("ActivityCalendars", Schema = "carma")]
    public class ActivityCalendar : AgencyCalendarFile
    {
        [Key]
        [Column("stream_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid CalendarID
        {
            get => base.CalendarID;
            set => base.CalendarID = value;
        }
    }
}
