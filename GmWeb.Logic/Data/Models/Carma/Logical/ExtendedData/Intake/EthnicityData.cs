using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;

namespace GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;

public record class EthnicityData
{
    public string Ethnicity { get; set; } = String.Empty;
    public CensusRace? Racial { get; set; }
    public string PrimaryLanguage { get; set; } = String.Empty;
    public string ExtendedLanguage { get; set; } = String.Empty;
    public string ChineseLanguage { get; set; } = String.Empty;
    public bool? ConversationalEnglish { get; set; }
}
