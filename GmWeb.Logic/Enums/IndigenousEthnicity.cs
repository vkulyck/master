using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Attributes;

namespace GmWeb.Logic.Enums;
public enum IndigenousEthnicity
{
    [Society(Country: "NZ", Adjective: "Maori", Race: CensusRace.Hawaiian)]
    Maori,
    [Society(Country: "MP", Adjective: "Chamorro", Race: CensusRace.Hawaiian)]
    Saipan,
    [Society(Country: "FSM", Adjective: "Caroline Islander", Race: CensusRace.Hawaiian)]
    CarolineIslands,
    [Society(Country: "NZ", Adjective: "Kermadec Islander", Race: CensusRace.Hawaiian)]
    KermadecIslands,
    [Society(Country: "USA", Adjective: "Hawaiian", Race: CensusRace.Hawaiian)]
    Hawaii,
    [Society(Country: "SLB", Adjective: "Santa Cruz Islander", Race: CensusRace.Hawaiian)]
    SantaCruzIslands
}
