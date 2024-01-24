using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IBaseCountryService = Countries.NET.ICountriesService;

using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Countries;

public abstract class CountryMatchingService<TOptions> : DatasetMatchingService<GmCountry, CountryMaps, TOptions>, IExportService<TOptions>
    where TOptions : CountryMatchingOptions
{
    private readonly CountryService _countryService;
    private readonly ILogger<CountryMatchingService<TOptions>> _logger;
    public override ILogger Logger => this._logger;

    public CountryMatchingService(CountryService countryService, TOptions options, ILoggerFactory factory, IHostEnvironment env) 
        : base(options, factory, env)
    {
        this._countryService = countryService;
        this._logger = factory.CreateLogger<CountryMatchingService<TOptions>>();
        this.InitializeMaps();
    }
    public CountryMatchingService(CountryService countryService, IOptions<TOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(countryService, options.Value, factory, env) { }

    public override string GetDisplayName(GmCountry data) => data.Name;
    public override string GetPrimaryKey(GmCountry data) => data.CCN3;
    protected override CountryMaps CreateMaps() => this._countryService.Maps;
}