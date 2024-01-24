using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GmWeb.Logic.Services.Datasets.Abstract;

using IBaseCountryService = Countries.NET.ICountriesService;
using Countries.NET;
using Countries.NET.Enums;

namespace GmWeb.Logic.Services.Datasets.Countries;
public class CountryService : DatasetService<GmCountry, CountryMaps, CountryOptions>
{
    private readonly IBaseCountryService _baseService;
    protected IBaseCountryService BaseService => _baseService;

    private readonly ILogger<CountryService> _logger;
    public override ILogger Logger => this._logger;

    private readonly List<GmCountry> _primaryCountries = new();
    public IReadOnlyList<GmCountry> PrimaryCountries => this._primaryCountries;

    public CountryService(IBaseCountryService baseService, CountryOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        _baseService = baseService;
        _logger = factory.CreateLogger<CountryService>();
        this.InitializeMaps();
        this._primaryCountries = this.LoadDataset(this.Options.PrimaryCountries);
    }
    public CountryService(IBaseCountryService baseService, IOptions<CountryOptions> options, ILoggerFactory factory, IHostEnvironment env) 
        : this(baseService, options.Value, factory, env) { }

    public override string GetDisplayName(GmCountry data) => data.Name;
    public override string GetPrimaryKey(GmCountry data) => data.CCN3;
    protected override CountryMaps CreateMaps() => CountryMaps.LoadSources(_baseService);
}