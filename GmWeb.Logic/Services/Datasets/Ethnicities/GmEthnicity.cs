using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Datasets.Abstract;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Utility.Attributes;
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Logic.Services.Datasets.Ethnicities;
public record class GmEthnicity : DataItem
{
    public string Code { get; }
    public string Name { get; }
    public string Adjective { get; }
    public GmCountry Country { get; }
    public EnumViewModel<IndigenousEthnicity> IndigenousEthnicity { get; }
    public SocietyAttribute Society { get; }
    private static GmEthnicity _OtherEthnicity;
    public static GmEthnicity OtherEthnicity => _OtherEthnicity;

    static GmEthnicity()
    {
        _OtherEthnicity = new GmEthnicity("Other", "Other/Not Listed", "000-OTHER");
    }

    public GmEthnicity(GmCountry country, CountryService countryService)
    {
        if(country is null)
            throw new ArgumentNullException(nameof(country));
        this.Country = country;
        this.Name = this.Country.Name;
        this.Adjective = this.Country.Adjective;
        this.Code = countryService.GetPrimaryKey(this.Country);
    }

    public GmEthnicity(EnumViewModel<IndigenousEthnicity> indigenousEthnicity, CountryService countryService)
    {
        this.IndigenousEthnicity = indigenousEthnicity;
        this.Society = this.IndigenousEthnicity.GetAttribute<SocietyAttribute>();
        this.Country = countryService.Maps[this.Society.CountryCode];
        this.Name = this.IndigenousEthnicity.Name;
        this.Adjective = this.Society.Adjective ?? this.Name;
        this.Code = $"{countryService.GetPrimaryKey(this.Country)}-INDG{indigenousEthnicity.ID}";
    }

    private GmEthnicity(string name, string adjective, string code)
    {
        this.Name = name;
        this.Adjective = adjective;
        this.Code = code;
    }

    public override bool Validate()
    {
        if (this == OtherEthnicity)
            return true;
        if (this.Country is null)
            return false;
        if (string.IsNullOrWhiteSpace(this.Adjective))
            return false;
        return true;
    }
}
