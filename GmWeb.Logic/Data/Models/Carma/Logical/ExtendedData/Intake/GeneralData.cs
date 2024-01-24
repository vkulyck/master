using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class GeneralData
{
    public string PreferredName { get; set; }
    public int? Gender { get; set; }
    public int? GenderPronouns { get; set; }
    public string Street { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public int? State { get; set; }
    public string ZipCode { get; set; }
}
