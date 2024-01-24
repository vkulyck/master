using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum MartialStatus
{
    [Display(Name = "Married")]
    Married,
    [Display(Name = "Living with a partner")]
    LivingPartner,
    [Display(Name = "Widowed")]
    Widowed,
    [Display(Name = "Divorced")]
    Divorced,
    [Display(Name = "Separated")]
    Separated,
    [Display(Name = "Never married")]
    NeverMarried
}
