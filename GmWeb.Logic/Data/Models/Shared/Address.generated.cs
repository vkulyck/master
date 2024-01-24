

using GmWeb.Logic.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Shared;
public partial class Address
{
    public string Recipient { get; set; }
    public string Organization { get; set; }
    public string StreetLine1 { get; set; }
    public string StreetLine2 { get; set; }
    public string StreetLine3 { get; set; }
    public string StreetLine4 { get; set; }
    // Apt, Suite, Unit, Box#, etc
    public string Premise { get; set; }
    [NotMapped]
    [JsonIgnore]
    public string Unit
    {
        get => this.Premise;
        set => this.Premise = value;
    }
    // City/Town
    public string Locality { get; set; }
    [NotMapped]
    [JsonIgnore]
    public string City
    {
        get => this.Locality;
        set => this.Locality = value;
    }
    // State/Province/Region
    public int AdministrativeAreaID { get; set; }
    [NotMapped]
    [JsonIgnore]
    public StateFipsCode StateID
    {
        get => (StateFipsCode)this.AdministrativeAreaID;
        set => this.AdministrativeAreaID = (int)value;
    }
    public CountryCode CountryID { get; set; } = CountryCode.United_States_of_America;
    public string PostalCode { get; set; }
}
