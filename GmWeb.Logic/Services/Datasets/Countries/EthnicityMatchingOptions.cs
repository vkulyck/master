using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Services.Deltas;
using Countries.NET;
using Countries.NET.Enums;
using Newtonsoft.Json;

namespace GmWeb.Logic.Services.Datasets.Countries;

public class EthnicityMatchingOptions : CountryMatchingOptions
{
    [JsonIgnore]
    public List<string> EthnicityQueries
    {
        get => base.Queries;
        set => base.Queries = value;
    }
}
