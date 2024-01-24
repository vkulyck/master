using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Enums;

[Flags]
public enum CensusRace
{
    None = 0,
    [Display(Description = "Hispanic, Latino, or Spanish Origin")]
    Hispanic =      0b000001,
    [Display(Description = "White")]
    White =         0b000010,
    [Display(Description = "Black or African American")]
    Black =         0b000100,
    [Display(Description = "American Indian and Alaska Native")]
    Indian =        0b001000,
    [Display(Description = "Far East Asian")]
    Asian =         0b010000,
    [Display(Description = "Native Hawaiian and Other Pacific Islander")]
    Hawaiian =      0b100000
}
