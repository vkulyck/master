using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Enums;

public enum GenderPronouns
{
    [Display(Name = "He/Him/His")]
    HeHimHis,
    [Display(Name = "She/Her/Her")]
    SheHerHer,
    [Display(Name = "They/Them/Their")]
    TheyThemTheir,
    [Display(Name = "Ey/Em/Eir")]
    EyEmEir,
    [Display(Name = "Ze/Hir/Hir")]
    ZeHirHir
}
