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
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Logic.Services.Datasets.Ethnicities;
public class EthnicityMaps : DataMaps<GmEthnicity>
{
    private static readonly List<(Func<GmEthnicity, string> Selector, Func<GmEthnicity, bool> Predicate)> Transforms = new()
    {
        (eth => eth.Name, default),
        (eth => eth.Adjective, default),
        (eth => eth.Country?.CCA2, default),
        (eth => eth.Country?.CCA3, default),
        (eth => eth.Country?.CCN3, default)
    };

    public EthnicityMaps(IEnumerable<GmEthnicity> sources)
        : base(sources, Transforms) { }

    public static EthnicityMaps LoadSources(EthnicityService ethnicityService, CountryService countryService)
    {
        List<GmEthnicity> loaded = new();
        foreach (var countryCode in ethnicityService.Options.EthnicityCountries)
        {
            var country = countryService.Maps[countryCode];
            loaded.Add(new GmEthnicity(country, countryService));
        }
        foreach (var evm in EnumExtensions.GetEnumViewModels<IndigenousEthnicity>())
            loaded.Add(new GmEthnicity(evm, countryService));
        loaded.Add(GmEthnicity.OtherEthnicity);
        return new EthnicityMaps(loaded);
    }
}
