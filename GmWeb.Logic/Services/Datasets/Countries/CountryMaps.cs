using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBaseCountryService = Countries.NET.ICountriesService;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Countries;
public class CountryMaps : DataMaps<GmCountry>
{
    private static readonly List<(Func<GmCountry, string> Selector, Func<GmCountry, bool> Predicate)> Transforms = new()
    {
        (co => co.CCA2, default),
        (co => co.CCA3, default),
        (co => co.CCN3, default),
        (co => co.DefaultDemonym, co => co.HasDemonyms),
        (co => co.Adjective, co => co.HasDemonyms),
        (co => co.Name, default),
    };

    public CountryMaps(IEnumerable<GmCountry> sources)
        : base(sources, Transforms) { }

    public static CountryMaps LoadSources(IBaseCountryService countryService)
    {
        List<GmCountry> loaded = new();
        foreach (var country in countryService.GetAll())
            loaded.Add(new GmCountry(country));
        return new CountryMaps(loaded);
    }
}
