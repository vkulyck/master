using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class OrientationData
{
    public int? SexOrientation { get; set; }
    public string OtherSexOrientation { get; set; }
}
