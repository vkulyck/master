using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum  Occupation
{
    [Display(Name = "Police officer")]
    FullTime,
    [Display(Name = "Fire fighter")]
    PartTime,
    [Display(Name = "Paramedic")]
    HomeMaker,
    [Display(Name = "Emergency Medical Technician (EMT)")]
    Retired,
    [Display(Name = "Correctional officer")]
    Disability,
    [Display(Name = "Other. Please specify")]
    Other
}
