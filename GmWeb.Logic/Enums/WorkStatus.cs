using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum  WorkStatus
{
    [Display(Name = "Working full time")]
    FullTime,
    [Display(Name = "Working part time")]
    PartTime,
    [Display(Name = "Full-time homemaker")]
    HomeMaker,
    [Display(Name = "Retired")]
    Retired,
    [Display(Name = "On disability")]
    Disability,
    [Display(Name = "On paid leave")]
    PaidLeave,
    [Display(Name = "On unpaid leave")]
    UnpaidLeave,
    [Display(Name = "Unemployed or laid off and looking for work")]
    LookForWork,
    [Display(Name = "Unemployed and not looking for work")]
    NotLookForWork,
    [Display(Name = "Other. Please specify")]
    Other
}
