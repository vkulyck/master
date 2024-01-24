using System;
using UserActivityStatus = GmWeb.Logic.Enums.UserActivityStatus;
using GmWeb.Web.Common.Models.Carma;

namespace GmWeb.Web.Api.Models.Common;

public class UserActivityDTO
{
    public int UserActivityID { get; set; }
    public Guid ActivityCalendarID { get; set; }
    public Guid ActivityEventID { get; set; }
    public Guid ActivityID { get; set; }
    public int RegistrantID { get; set; }
    public UserDTO Registrant { get; set; }
    public int RegistrarID { get; set; }
    public UserDTO Registrar { get; set; }
    public DateTime ActivityStart { get; set; }
    public DateTime DateRegistered { get; set; }
    public DateTime? DateConfirmed { get; set; }
    public UserActivityStatus Status { get; set; } = UserActivityStatus.Registered;
}
