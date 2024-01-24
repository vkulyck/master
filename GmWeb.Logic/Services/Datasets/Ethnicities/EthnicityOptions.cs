using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Datasets.Ethnicities;
public class EthnicityOptions : ServiceOptions
{
    public List<string> EthnicityCountries { get; set; } = new();
}
