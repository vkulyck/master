using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Education
{
    [Display(Name = "5 years K-5")]
    FiveYears,
    [Display(Name = "12 years K-6 to High School dipolma/GED")]
    TwelveYears,
    [Display(Name = "14 years Associate's degree/certificate")]
    FourteenYears,
    [Display(Name = "16 years Bachelor's degree")]
    SixteenYears,
    [Display(Name = "18 years Master's degree")]
    EighteenYears,
    [Display(Name = "19+ year Ph.D. or advanced professional degree")]
    NineteenYears,
    [Display(Name = "Postdoctoral")]
    Postdoctoral
}
