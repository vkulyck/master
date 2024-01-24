using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Datasets.Abstract;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Logic.Services.Datasets.Races;
public class RaceMaps : DataMaps<GmRace>
{
    private static readonly List<(Func<GmRace, string> Selector, Func<GmRace, bool> Predicate)> Transforms = new()
    {
        (eth => eth.Name, default),
        (eth => eth.Code, default)
    };

    public RaceMaps(IEnumerable<GmRace> sources)
        : base(sources, Transforms) { }

    public static RaceMaps LoadSources(IReadOnlyDictionary<string,CensusRace> countryRaces, EthnicityService ethnicityService)
    {
        
        List<GmRace> loaded = new();
        foreach(var eth in ethnicityService.Sources)
        {
            if (eth.Society is not null)
                loaded.Add(new GmRace(eth, eth.Society));
            else
            {
                var ccn3 = eth.Country?.CCN3.ToString();
                if (string.IsNullOrWhiteSpace(ccn3))
                    continue;
                if (!countryRaces.ContainsKey(ccn3))
                    continue;
                var race = countryRaces[ccn3];
                loaded.Add(new GmRace(eth, race));
            }
        }
        return new RaceMaps(loaded);
    }
}
