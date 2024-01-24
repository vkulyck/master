using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CensusRace = GmWeb.Logic.Enums.CensusRace;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Races;
public class RaceService : DatasetService<GmRace, RaceMaps, RaceOptions>
{
    private readonly EthnicityService _ethnicityService;
    private readonly ILogger<RaceService> _logger;
    public override ILogger Logger => this._logger;
    public IReadOnlyDictionary<string,CensusRace> CountryRaces { get; private set; }

    public RaceService(EthnicityService ethnicityService, RaceOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        _ethnicityService = ethnicityService;
        _logger = factory.CreateLogger<RaceService>();
        this.CountryRaces = this.Options.CountryRaces;
        this.InitializeMaps();
    }
    public RaceService(EthnicityService ethnicityService, IOptions<RaceOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(ethnicityService, options.Value, factory, env) { }

    public override string GetDisplayName(GmRace data) => data.Name;
    public override string GetPrimaryKey(GmRace data) => data.Code;
    protected override RaceMaps CreateMaps() => RaceMaps.LoadSources(this.CountryRaces, this._ethnicityService);
}