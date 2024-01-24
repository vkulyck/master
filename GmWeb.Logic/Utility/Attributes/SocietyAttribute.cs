using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Race = GmWeb.Logic.Enums.CensusRace;

namespace GmWeb.Logic.Utility.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SocietyAttribute : Attribute
{
    public Race Race { get; set; }
    public string Adjective { get; protected set; }
    public string CountryCode { get; protected set; }
    public string LanguageCode { get; protected set; }
    public SocietyAttribute(string Country, Race Race, string Adjective = null, string Language = null)
    {
        this.Race = Race;
        this.CountryCode = Country;
        this.Adjective = Adjective;
        this.LanguageCode = Language;
    }
}
