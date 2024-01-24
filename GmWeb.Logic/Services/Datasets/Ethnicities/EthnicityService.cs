using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Ethnicities;
public class EthnicityService : DatasetService<GmEthnicity, EthnicityMaps, EthnicityOptions>
{
    private readonly CountryService _countryService;
    protected CountryService CountryService => _countryService;

    private readonly ILogger<EthnicityService> _logger;
    public override ILogger Logger => this._logger;

    public static string OtherEthnicityKey => GmEthnicity.OtherEthnicity.Code;

    public EthnicityService(CountryService countryService, EthnicityOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        _countryService = countryService;
        _logger = factory.CreateLogger<EthnicityService>();
        this.InitializeMaps();
    }
    public EthnicityService(CountryService countryService, IOptions<EthnicityOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(countryService, options.Value, factory, env) { }

    public override string GetDisplayName(GmEthnicity data) => data.Name;
    public override string GetPrimaryKey(GmEthnicity data) => data.Code;
    protected override EthnicityMaps CreateMaps() => EthnicityMaps.LoadSources(this, _countryService);
}