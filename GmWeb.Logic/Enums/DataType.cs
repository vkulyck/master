using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums
{
    public enum DataType
    {
        [Display(ShortName = "Integer")]
        N = 1,
        [Display(ShortName = "Date")]
        D = 2,
        [Display(ShortName = "List")]
        L = 3,
        [Display(ShortName = "Boolean")]
        B = 4,
        [Display(ShortName = "String")]
        S = 5
    }
}
