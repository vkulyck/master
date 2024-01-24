using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GmWeb.Logic.Services.Datasets.Abstract;
public abstract class DatasetService<TData, TDataMaps, TOptions>
    where TData : DataItem
    where TOptions : ServiceOptions
    where TDataMaps : DataMaps<TData>
{
    private readonly TOptions _options;
    private readonly ILogger<DatasetService<TData, TDataMaps, TOptions>> _logger;
    private readonly IHostEnvironment _env;
    protected TDataMaps _maps;
    public TOptions Options => this._options;
    public virtual ILogger Logger => this._logger;
    public IHostEnvironment Env => this._env;
    public TDataMaps Maps => this._maps;
    public IReadOnlyList<TData> Sources => this.Maps.All;

    public DatasetService(TOptions options, ILoggerFactory factory, IHostEnvironment env)
    {
        _options = options;
        _logger = factory.CreateLogger<DatasetService<TData, TDataMaps, TOptions>>();
        _env = env;
    }
    public DatasetService(IOptions<TOptions> options, ILoggerFactory factory, IHostEnvironment env) 
        : this(options.Value, factory, env) { }

    public abstract string GetDisplayName(TData data);
    public abstract string GetPrimaryKey(TData data);
    protected void InitializeMaps() { _maps = this.CreateMaps(); }
    protected abstract TDataMaps CreateMaps();
    protected List<TData> LoadDataset(IEnumerable<string> keyList)
        => this.LoadDataset(keyList.Select(key => new string[] { key }));
    protected List<TData> LoadDataset(IEnumerable<string[]> keyLists)
    {
        var loaded = new List<TData>();
        foreach (var keyList in keyLists)
        {
            foreach (var key in keyList)
            {
                if (this.Maps.TryGetValue(key, out var result))
                {
                    loaded.Add(result);
                    break;
                }
            }
        }
        return loaded;
    }
}