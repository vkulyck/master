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

using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Services.Datasets.Languages;

public record struct MatchedLanguage
{
    public string Query { get; }
    public CompareMethod CompareMethod { get; }
    public int MatchIndex { get; }
    private Tags.Language Language { get; }
    public string Name { get; set; }
    public string Tag => this.Language.Subtag;
    public double? Score { get; set; }
    public double? Confidence
    {
        get => 1 - this.Score;
        set => this.Score = 1 - value;
    }

    public MatchedLanguage(string query, CompareMethod compareMethod, Tags.Language language, int matchIndex, string name, double? score = null)
    {
        this.Query = query;
        this.CompareMethod = compareMethod;
        this.Language = language;
        this.MatchIndex = matchIndex;
        this.Name = name;
        this.Score = score;
    }
    public MatchedLanguage(string query, CompareMethod compareMethod, Tags.Language language, int matchIndex, int descriptionIndex, double? score = null)
        : this(query, compareMethod, language, matchIndex, language.Descriptions[descriptionIndex], score)
    { }
}
