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

using GmWeb.Logic.Enums;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Services.Datasets.Abstract;

public abstract class DatasetMatchingService<TData, TDataMaps, TOptions> : DatasetService<TData, TDataMaps, TOptions>, IExportService<TOptions>
    where TData : DataItem
    where TDataMaps : DataMaps<TData>
    where TOptions : DatasetMatchingOptions
{
    private readonly DeltaService _delta;
    private readonly ILogger<DatasetMatchingService<TData, TDataMaps, TOptions>> _logger;
    public override ILogger Logger => this._logger;
    protected IExportService<TOptions> ExportBase => ((IExportService<TOptions>)this);

    public record class MatchedData
    {
        public string Query { get; }
        public CompareMethod CompareMethod { get; }
        public int MatchIndex { get; }
        public TData Data { get; }
        public string Name { get; set; }
        public string Key { get; set; }
        public double? Score { get; set; }
        public double? Confidence
        {
            get => 1 - this.Score;
            set => this.Score = 1 - value;
        }

        public MatchedData(string query, CompareMethod compareMethod, TData data, int matchIndex, string name, string key, double? score = null)
        {
            this.Query = query;
            this.CompareMethod = compareMethod;
            this.Data = data;
            this.MatchIndex = matchIndex;
            this.Name = name;
            this.Key = key;
            this.Score = score;
        }
    }


    public DatasetMatchingService(TOptions options, ILoggerFactory factory, IHostEnvironment env)
        : base(options, factory, env)
    {
        this._delta = new DeltaService(this.Options.Deltas);
        _logger = factory.CreateLogger<DatasetMatchingService<TData, TDataMaps, TOptions>>();
    }
    public DatasetMatchingService(IOptions<TOptions> options, ILoggerFactory factory, IHostEnvironment env)
        : this(options.Value, factory, env) { }

    #region Abstract Methods
    protected abstract string GetMatchingField(TData data);
    protected abstract TData LookupByMatchingField(string value);
    #endregion

    #region Result Model

    protected IEnumerable<MatchedData> CreateExactMatches(string query, TData Data)
        => CreateMatches(query, new[] { (query, Data, default(double?)) }, CompareMethod.Exact);
    protected IEnumerable<MatchedData> CreateContainedMatches(string query, IEnumerable<(string Name, TData Data)> Countries)
        => CreateMatches(query, Countries.Select(x => (x.Name, x.Data, default(double?))), CompareMethod.Contains);
    protected IEnumerable<MatchedData> CreateFuzzyMatches(string query, IEnumerable<(string Name, TData Data, double Score)> Countries)
        => CreateMatches(query, Countries.Select(x => (x.Name, x.Data, (double?)x.Score)), CompareMethod.Fuzzy);
    private IEnumerable<MatchedData> CreateMatches(string query, IEnumerable<(string Name, TData Data, double? Score)> Countries, CompareMethod compareMethod)
    {
        int matchIndex = 0;
        foreach (var nl in Countries)
        {
            yield return new MatchedData(query, compareMethod, nl.Data, matchIndex++, nl.Name, this.GetPrimaryKey(nl.Data), nl.Score);
        }
    }

    protected IEnumerable<MatchedData> FindMatches(IEnumerable<string> queries)
    {
        var matchMethods = new List<Func<string, IEnumerable<MatchedData>>>
        {
            this.FindExactMatches,
            this.FindContainedMatches,
            this.FindFuzzyMatches
        };
        foreach (var query in queries)
        {
            bool any = false;
            foreach (var method in matchMethods)
            {
                foreach (var ml in method(query))
                {
                    any = true;
                    yield return ml;
                }
                if (any)
                    break;
            }
        }
    }
    #endregion

    #region Find Methods
    protected IEnumerable<MatchedData> FindExactMatches(string query)
    {
        if (this.Maps.TryGetValue(query, out TData exactCountry))
        {
            foreach (var ml in this.CreateExactMatches(query, exactCountry))
            {
                _logger.LogInformation($"EXACT[0]: Query={query}, Match='{ml.Name}', Tag={ml.Key}");
                yield return ml;
            }
        }
    }
    protected IEnumerable<MatchedData> FindContainedMatches(string query)
    {
        var contained = this.Sources
            .Select(co => this.GetMatchingField(co))
            .Where(value => value.Contains(query))
            .Select(value => (value, this.LookupByMatchingField(value)))
            .ToList()
        ;
        if (contained.Count > 0)
        {
            foreach (var ml in this.CreateContainedMatches(query, contained))
            {
                _logger.LogInformation($"CONTN[{ml.MatchIndex:D2}]: Query={query}, Match='{ml.Name}', Tag={ml.Key}");
                yield return ml;
            }
        }
    }
    protected IEnumerable<MatchedData> FindFuzzyMatches(string query)
    {
        var fuzzySelector = (int loc) => (string candidate) =>
        {
            var matchLoc = _delta.match_bitap(candidate, query, loc, out double score);
            return (matchLoc, score);
        };
        var fuzzyResults = this.Sources.Select(co => this.GetMatchingField(co))
            .SelectMany(value => Enumerable.Range(0, value.Length).Select(i => (value, i)))
            .Select(valueLoc =>
            {
                (int MatchLoc, double MatchScore) locScore = fuzzySelector(valueLoc.i)(valueLoc.value);
                (string Value, int QueryLoc, int MatchLoc, double MatchScore) result = (valueLoc.value, valueLoc.i, locScore.MatchLoc, locScore.MatchScore);
                return result;
            })
            .Where(x => x.MatchLoc >= 0)
            .OrderBy(x => x.MatchScore)
            .Select(klr => new { Value = klr.Value, StartLoc = klr.QueryLoc, MatchLoc = klr.MatchLoc, MatchScore = klr.MatchScore })
            .ToList()
        ;
        var uniques = fuzzyResults
            .GroupBy(fr => fr.Value)
            .Select(frg =>
            {
                var bestResult = frg.OrderBy(fr => fr.MatchScore).FirstOrDefault();
                return new
                {
                    Value = frg.Key,
                    Score = bestResult.MatchScore,
                    StartLoc = bestResult.StartLoc,
                    MatchLoc = bestResult.MatchLoc,
                    Data = this.LookupByMatchingField(frg.Key)
                };
            })
            .OrderBy(fr => fr.Score)
            .ToList()
        ;
        for (int matchIndex = 0; matchIndex < uniques.Count; matchIndex++)
        {
            var fr = uniques[matchIndex];
            int
                preIndex = fr.MatchLoc,
                postIndex = fr.MatchLoc + query.Length - 1
            ;
            bool truncate = postIndex >= fr.Value.Length - 1;
            string
                preMatch = fr.Value.Substring(0, fr.MatchLoc),
                match = truncate ? fr.Value.Substring(fr.MatchLoc) : fr.Value.Substring(fr.MatchLoc, query.Length),
                postMatch = truncate ? "" : fr.Value.Substring(postIndex)
            ;
            var matchString = $"{preMatch}({match}){postMatch}";
            _logger.LogInformation($"FUZZY[{matchIndex:D2}]: Query={query}, Match='{matchString}', Value={fr.Value}, Score={fr.Score:N2}");
        }
        var countries = uniques
            .Select(u => (u.Value, u.Data, u.Score))
            .ToList()
        ;
        foreach (var ml in this.CreateFuzzyMatches(query, countries))
            yield return ml;
    }
    #endregion


    #region Export
    public async Task Export(string OutputFilename = null, string OutputDirectory = null, string OutputPath = null)
    {
        OutputPath = this.ExportBase.GetExporOutputPath(OutputFilename, OutputDirectory, OutputPath);
        using var writer = new StreamWriter(OutputPath);
        var matches = this.FindMatches(this.Options.Queries);
        await writer.WriteLineAsync("Query, Value, Key, Name, Match#, Method, Score");
        foreach (var match in matches)
            await writer.WriteLineAsync(string.Join(",",
                match.Query, match.Name, match.Key,
                GetDisplayName(match.Data), match.MatchIndex + 1, match.CompareMethod, match.Confidence
            ));
    }
    #endregion
}