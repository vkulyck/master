using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GmWeb.Logic.Services.Datasets.Countries;
public class EthnicityMatchingService : CountryMatchingService<EthnicityMatchingOptions>
{
    protected override string GetMatchingField(GmCountry data) => data.Adjective;
    protected override GmCountry LookupByMatchingField(string value) => this.Maps.TryGetValue(value, out var result) ? result : null;

    public EthnicityMatchingService(CountryService countryService, IOptions<EthnicityMatchingOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : base(countryService, options, factory, env)
    { }
}
