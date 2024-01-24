using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class FamilyData
{
    public int? AllChildren { get; set; }
    public int? LivingChildren { get; set; }
    public int? PeopleCount { get; set; }
}
