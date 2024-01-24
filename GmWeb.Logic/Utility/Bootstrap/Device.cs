using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Utility.Bootstrap
{
    public enum Device
    {
        General = 0,
        [Display(ShortName = "xs")]
        Phone = 1,
        [Display(ShortName = "md")]
        Tablet = 2,
        [Display(ShortName = "lg")]
        Desktop = 3,
        [Display(ShortName = "xl")]
        Television = 4
    }
}
