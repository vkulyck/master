using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GmWeb.Logic.Utility.Extensions.Enums;
using GmWeb.Logic.Utility.Attributes;

namespace GmWeb.Web.RHI.Utility;

public enum IntakePageType
{
    [Icon("user-circle")]
    [Display(Name = "General Information")]
    General = 0,
    [Icon("users")]
    [Display(Name = "Race/Ethnicity")]
    Ethnicity,
    [Icon("globe")]
    [Display(Name = "Nationality")]
    Nationality,
    [Icon("praying-hands")]
    [Display(Name = "Religion")]
    Religion,
    [Icon("heart")]
    [Display(Name = "Sexual Orientation")]
    Orientation,
    [Icon("gem")]
    [Display(Name = "Marital Status")]
    Marital,
    [Icon("baby")]
    [Display(Name = "Family MakeUp")]
    Family,
    [Icon("file")]
    [Display(Name = "Education and Work")]
    Career,
    [Icon("hand-holding-medical")]
    [Display(Name = "Insurance Coverage")]
    Insurance
}
