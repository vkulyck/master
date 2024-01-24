using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Racial
{
    [Display(Name = "	Asian non-Hispanic or Latino	")]
    AsianNonHispanic,
    [Display(Name = "	White and Hispanic or Latino	")]
    WhiteAndHispanic,
    [Display(Name = "	Native Hawaiian and Other Pacific Islander non-Hispanic or Latino	")]
    OtherPacificNonHispanic,
    [Display(Name = "	Black or African America and Hispanic or Latino	")]
    BlackAmericaHispanic,
    [Display(Name = "	Black or African America non-Hispanic or Latino	")]
    BlackAmericaNonHispanic,
    [Display(Name = "	American Indian and Alaska Native and Hispanic or Latino	")]
    IndianAlaskHispanic,
    [Display(Name = "	Native Hawaiian and Other Pacific Islander and Hispanic or Latino	")]
    HawaiianPacificHispanic,
    [Display(Name = "	American Indian and Alaska Native non-Hispanic or Latino	")]
    IndianAlaskaNonHispanic,
    [Display(Name = "	Other	")]
    Other
}
