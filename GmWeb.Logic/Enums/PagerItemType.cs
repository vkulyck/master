using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Attributes;

namespace GmWeb.Logic.Enums;
public enum PagerItemType 
{ 
    [Icon("fast-backward")]
    First,
    [Icon("backward")]
    [Display(ShortName = "Prev")]
    Previous,
    Target,
    [Icon("forward")]
    Next,
    [Icon("fast-forward")]
    Last 
};