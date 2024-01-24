using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum SexualOrientation
{
    [Display(Name = "Straight")]
    Straight,
    [Display(Name = "Lesbian")]
    Lesbian,
    [Display(Name = "Gay")]
    Gay,
    [Display(Name = "Bisexual")]
    Bisexual,
    [Display(Name = "Asexual")]
    Asexual,
    [Display(Name = "Questioning")]
    Questioning,
    [Display(Name = "I rather not say")]
    NotSay,
    [Display(Name = "Not Listed")]
    NotListed
}
