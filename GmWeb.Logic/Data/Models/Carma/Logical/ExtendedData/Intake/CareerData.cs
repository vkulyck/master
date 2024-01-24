using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class CareerData
{
    public int? CurrentOccupation { get; set; }
    public string OtherOccupation { get; set; }
    public int? Education { get; set; }
    public int? Job { get; set; }
    public int? Income { get; set; }
    public int? WorkStatus { get; set; }
    public string OtherWork { get; set; }
}
