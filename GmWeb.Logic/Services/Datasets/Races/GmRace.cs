using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Datasets.Abstract;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using GmWeb.Logic.Utility.Attributes;
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Logic.Services.Datasets.Races;
public record class GmRace : DataItem
{
    public GmEthnicity Ethnicity { get; }
    public CensusRace Value { get; }
    public string Name { get; }
    public string Code { get; }

    public GmRace(GmEthnicity ethnicity, SocietyAttribute society)
        : this(ethnicity, society.Race) { }
    public GmRace(GmEthnicity ethnicity, CensusRace race)
    {
        var evm = new EnumViewModel<CensusRace>(race);
        this.Ethnicity = ethnicity;
        this.Name = evm.Name;
        this.Code = evm.ID.ToString();
        this.Value = evm.Value;
    }

    public override bool Validate()
    {
        if (!this.Ethnicity.Validate())
            return false;
        return true;
    }
}
