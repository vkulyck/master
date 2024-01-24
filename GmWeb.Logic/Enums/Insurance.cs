using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Insurance
{
        [Display(Name = "Uninsured")]
        Uninsured,
        [Display(Name = "Insured but paying out of pocket")]
        PocketInsurance,
        [Display(Name = "Employer health insurance")]
        EmployerInsurance,
        [Display(Name = "Private-paid health insurance")]
        PrivateInsurance,
        [Display(Name = "Military health insurance")]
        MilitaryInsurance,
        [Display(Name = "Medi-Cal")]
        Medical,
        [Display(Name = "Medicare")]
        Medicare,
        [Display(Name = "Health saving account (HSA)")]
        HSA,
        [Display(Name = "Other. Please specify")]
        Other
}
