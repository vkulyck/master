using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainCountryData = Countries.NET.Country;
using Demonyms = Countries.NET.DemonymsGender;
using CountryName = Countries.NET.CountryName;
using ExtendedCountryData = ExtendedIsoCountries.Country;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Countries;
public record class GmCountry : DataItem
{
    private readonly MainCountryData _main;
    private readonly ExtendedCountryData _extended;
    private readonly Demonyms _demonyms;
    private readonly CountryName _localizedName;
    public GmCountry(MainCountryData main)
    {
        this._main = main;
        if (!int.TryParse(this.CCN3, out int result))
            return;
        this.ICCN3 = result;
        this._extended = ExtendedCountryData.GetByNumericCode(this.ICCN3);
        this._localizedName = this.GetLocalizedName(this._main);
        this._demonyms = this.GetDemonyms(this._main);
    }
    protected virtual CountryName GetLocalizedName(MainCountryData main)
    {
        var localizable = main.Name;
        var language = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
        if (localizable.TryGetValue(language, out var localName))
            return localName;
        if(localizable.TryGetValue(Languages.LanguageService.DefaultLanguageCode, out var defaultLocalized))
            return defaultLocalized;
        return null;
    }
    protected virtual Demonyms GetDemonyms(MainCountryData main)
    {
        var localizable = main.Demonyms;
        var language = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
        if (localizable.TryGetValue(language, out var localeDemonyms))
            return localeDemonyms;
        if(localizable.TryGetValue(Languages.LanguageService.DefaultLanguageCode, out var defaultDemonyms))
            return defaultDemonyms;
        return null;
    }
    public override bool Validate()
    {
        if (this._extended == null)
            return false;
        if (this._localizedName == null)
            return false;
        var required = new string[] { this.CCA2, this.CCA3, this.CCN3, this.Name, this.Adjective };
        foreach (var r in required)
            if (string.IsNullOrWhiteSpace(r))
                return false;
        return true;
    }

    /// <summary>
    /// The two-letter ISO country code; aka ISO 3166-1 alpha-2. These codes are typically used for the Internet's country code top-level domains as well as web translation frameworks.
    /// </summary>
    public string CCA2 => this._main.CCA2;

    /// <summary>
    /// The three-letter ISO country code; aka ISO 3166-1 alpha-3. Three-letter country codes which allow a better visual association between the codes and the country names than the alpha-2 codes.
    /// </summary>
    public string CCA3 => this._main.CCA3;

    /// <summary>
    /// The three-digit ISO country code; aka 'ISO 3166-1 numeric'.
    /// </summary>
    public string CCN3 => this._main.CCN3;
    public int ICCN3 { get; }

    public bool HasDemonyms => this._demonyms is not null;
    /// <summary>
    /// The localized male demonym; used to refer to male residents of the country.
    /// </summary>
    public string MaleDemonym => this._demonyms?.Male;

    /// <summary>
    /// The localized female demonym; used to refer to female residents of the country.
    /// </summary>
    public string FemaleDemonym => this._demonyms?.Female;

    /// <summary>
    /// An alias for <see cref="MaleDemonym">MaleDemonym</see>.
    /// </summary>
    public string DefaultDemonym => this.MaleDemonym;
    public string Name => this._localizedName?.Common;
    public string OfficialName => this._localizedName?.Official;

    /// <summary>
    /// The adjective used to refer to entities originating from the instance country; often equivalent to (or used interchangeably with) the demonym(s).
    /// </summary>
    public string Adjective => this._extended.Adjective;

}
