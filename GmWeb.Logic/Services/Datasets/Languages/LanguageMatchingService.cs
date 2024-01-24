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
using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Languages;

public abstract class LanguageMatchingService<TOptions> : DatasetMatchingService<GmLanguage, LanguageMaps, TOptions>, IExportService<TOptions>
    where TOptions : LanguageMatchingOptions
{
    private readonly LanguageService _languageService;
    private readonly ILogger<LanguageMatchingService<TOptions>> _logger;
    public override ILogger Logger => this._logger;

    public LanguageMatchingService(LanguageService languageService, TOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        this._languageService = languageService;
        this._logger = factory.CreateLogger<LanguageMatchingService<TOptions>>();
        this.InitializeMaps();
    }
    public LanguageMatchingService(LanguageService languageService, IOptions<TOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(languageService, options.Value, factory, env) { }

    public override string GetDisplayName(GmLanguage data) => data.Name;
    public override string GetPrimaryKey(GmLanguage data) => data.Code;
    protected override LanguageMaps CreateMaps() => this._languageService.Maps;
}