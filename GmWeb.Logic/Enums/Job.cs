using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Job
{
    [Display(Name = "Senior Management/Leadership (CEO, Executive Director, Senior/Executive VP, Chief level positions, etc)")]
    SeniorManag,
    [Display(Name = "Upper management (Director, VP, Duty chief, etc)")]
    UpperManag,
    [Display(Name = "Middle management")]
    MiddleManag,
    [Display(Name = "Lower management")]
    LoverManag,
    [Display(Name = "Leader/foreman")]
    Foreman,
    [Display(Name = "Contributor/team member")]
    Contributor
}
