using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class NationalityData
{
    public int? Country { get; set; }
    public string NationalState { get; set; }
    public int? USState { get; set; }
    public string Town { get; set; }
    public int? YearsInUS { get; set; }
    public int? AgeInUS { get; set; }
}
