using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CensusRace = GmWeb.Logic.Enums.CensusRace;

namespace GmWeb.Logic.Services.Datasets.Races;
public class RaceOptions : ServiceOptions
{
    public Dictionary<string, CensusRace> CountryRaces { get; set; } = new();
}
