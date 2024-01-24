using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algenta.Globalization.LanguageTags;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Languages;
public record class GmLanguage : DataItem
{
    public LanguageTag Source { get; }
    public Language Language { get; }
    /// <summary>
    /// The ISO standardized language name in English.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The ISO 639-1 language code.
    /// </summary>
    public string Alpha2 { get; }
    /// <summary>
    /// The ISO 639-2/B (Bibliographic) language code.
    /// </summary>
    public string Alpha3B { get; }
    /// <summary>
    /// The ISO 639-2/T (Terminological) language code.
    /// </summary>
    public string Alpha3T { get; }
    /// <summary>
    /// The language code created by coalescing Alpha2, Alpha3T, and then Alpha3B.
    /// </summary>
    public string Code { get; }
    public GmLanguage(string[] codeList)
    {
        if (codeList.Length != 3)
            throw new ArgumentException($"Expected a list of language codes in 	ISO 639-1, ISO 639-2/B, and ISO 639-2/T formats, respectively.");
        var nullified = codeList.Select(x => string.IsNullOrWhiteSpace(x) ? default : x).ToList();
        foreach (var code in nullified)
        {
            if (!TagRegistry.TryParse(code, out var tag))
                continue;
            this.Source = tag;
        }
        if (this.Source is null)
            return;
        this.Language = TagRegistry.GetLanguage(this.Source.Language);
        this.Name = this.Language.Descriptions.FirstOrDefault();
        this.Alpha2 = nullified[0];
        this.Alpha3B = nullified[1];
        this.Alpha3T = nullified[2];
        this.Code = this.Alpha2 ?? this.Alpha3T ?? this.Alpha3B;
    }

    public override bool Validate()
    {
        if (this.Source is null)
            return false;
        return true;
    }
}
