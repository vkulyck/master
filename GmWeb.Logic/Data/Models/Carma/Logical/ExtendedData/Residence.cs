using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Address = GmWeb.Logic.Data.Models.Shared.Address;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData;

public class Residence
{
    public string BuildingCode { get; set; }
    public string UnitNumber { get; set; }
    public Address Address { get; set; } = new();
}
