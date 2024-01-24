using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Algenta.Globalization.LanguageTags;
using Tags = Algenta.Globalization.LanguageTags;

using GmWeb.Logic.Services.Datasets.Abstract;

namespace GmWeb.Logic.Services.Datasets.Languages;

public class LanguageService : DatasetService<GmLanguage, LanguageMaps, LanguageOptions>
{
    public static readonly string DefaultLanguageCode = "eng";
    private readonly ILogger<LanguageService> _logger;
    private readonly List<GmLanguage> _primaryLanguages = new();
    private readonly List<GmLanguage> _extendedLanguages = new();
    private readonly List<GmLanguage> _chineseLanguages = new();

    public override ILogger Logger => this._logger;
    public IReadOnlyList<GmLanguage> PrimaryLanguages => this._primaryLanguages;
    public IReadOnlyList<GmLanguage> ExtendedLanguages => this._extendedLanguages;
    public IReadOnlyList<GmLanguage> ChineseLanguages => this._chineseLanguages;

    public LanguageService(LanguageOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        this._logger = factory.CreateLogger<LanguageService>();
        this.InitializeMaps();
        this._primaryLanguages = this.LoadDataset(this.Options.PrimaryLanguages);
        this._extendedLanguages = this.LoadDataset(this.Options.ExtendedLanguages);
        this._chineseLanguages = this.LoadDataset(this.Options.ChineseLanguages);
    }
    public LanguageService(IOptions<LanguageOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(options.Value, factory, env) { }

    public override string GetDisplayName(GmLanguage data) => data.Name;
    public override string GetPrimaryKey(GmLanguage data) => data.Code;
    protected override LanguageMaps CreateMaps() => LanguageMaps.LoadSources(this.Options.PrimaryLanguages);
}