using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums
{
    public enum IncomeLevel
    {
        [Display(Description = "Extremely Low Income")]
        ExtremeLow = 1,
        [Display(Description = "Low Income")]
        Low = 2,
        [Display(Description = "Moderate Income")]
        Moderate = 3,
        [Display(Description = "Above Moderate Income")]
        AboveModerate = 4
    }
}