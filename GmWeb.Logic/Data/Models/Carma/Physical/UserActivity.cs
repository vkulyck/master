using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Carma
{
    [Table("UserActivities", Schema = "carma")]
    public class UserActivity : BaseDataModel
    {
        [Key]
        public int UserActivityID { get; set; }
        [ForeignKey("ActivityCalendar")]
        public Guid ActivityCalendarID { get; set; }
        [ForeignKey("ActivityCalendarID")]
        public virtual ActivityCalendar ActivityCalendar { get; set; }
        public Guid ActivityEventID { get; set; }
        public Guid ActivityID { get; set; }
        [ForeignKey("Registrant")]
        public int RegistrantID { get; set; }
        [ForeignKey("RegistrantID")]
        public virtual User Registrant { get; set; }
        [ForeignKey("Registrar")]
        public int RegistrarID { get; set; }
        [ForeignKey("RegistrarID")]
        public virtual User Registrar { get; set; }
        public DateTime ActivityStart { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime? DateConfirmed { get; set; }
        [SqlDataType(System.Data.SqlDbType.TinyInt)]
        public UserActivityStatus Status { get; set; } = UserActivityStatus.Registered;
    }
}
