using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algenta.Globalization.LanguageTags;
using Tags = Algenta.Globalization.LanguageTags;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Languages;
public class LanguageMaps : DataMaps<GmLanguage>
{
    private static readonly List<(Func<GmLanguage, string> Selector, Func<GmLanguage, bool> Predicate)> Transforms = new()
    {
        (lang => lang.Alpha2, lang => lang.Alpha2 is not null),
        (lang => lang.Alpha3T, lang => lang.Alpha3T is not null),
        (lang => lang.Alpha3B, lang => lang.Alpha3B is not null),
        (lang => lang.Name, lang => lang.Name is not null)
    };

    public LanguageMaps(IEnumerable<GmLanguage> sources)
        : base(sources, Transforms) { }

    public static LanguageMaps LoadSources(List<string[]> sources)
    {
        List<GmLanguage> loaded = new();
        foreach(var codeList in sources)
            loaded.Add(new GmLanguage(codeList));
        return new LanguageMaps(loaded);
    }
}
