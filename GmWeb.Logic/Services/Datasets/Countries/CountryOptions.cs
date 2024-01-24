using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Services.Datasets.Countries;

public class CountryOptions : ServiceOptions
{
    public List<string> PrimaryCountries { get; set; } = new();
}
