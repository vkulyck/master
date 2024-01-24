using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums
{
    public enum HeadOfHousehold
    {
        [Display(Name = "Self")]
        SingleWithout = 0,
        [Display(Name = "Couple")]
        DualWithout = 1,
        [Display(Name = "Single Head of Household")]
        SingleWith = 2,
        [Display(Name = "Dual Head of Household")]
        DualWith = 3
    }
}
