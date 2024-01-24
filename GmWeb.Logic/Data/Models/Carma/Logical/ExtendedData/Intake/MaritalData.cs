using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class MaritalData
{
    public int? MartialStatus { get; set; }
    public int? Years { get; set; }
    public int? Month { get; set; }
}
