using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Datasets.Abstract;

public static class DataMapsExtensions
{
    public static DataMaps<TData> ToDataMaps<TData>(this IEnumerable<TData> sources, params Func<TData, string>[] selectors)
        where TData : DataItem
        => new DataMaps<TData>(sources, selectors);
}
public class DataMaps<TData> : IReadOnlyDictionary<string, TData>
    where TData : DataItem
{

    private readonly List<DataMap<string, TData>> _maps = new();
    private readonly Dictionary<string, TData> _combined = new();
    private readonly List<TData> _all = new();
    public IReadOnlyList<TData> All => this._all;

    public DataMaps(IEnumerable<TData> sources, params Func<TData, string>[] selectors)
        : this(sources, selectors.AsEnumerable()) { }
    public DataMaps(IEnumerable<TData> sources, IEnumerable<Func<TData, string>> selectors)
        : this(sources, selectors.Select(s => (s, default(Func<TData, bool>)))) { }
    public DataMaps(IEnumerable<TData> sources, params (Func<TData, string> Selector, Func<TData, bool> Predicate)[] transforms)
        : this(sources, transforms.AsEnumerable()) { }
    public DataMaps(IEnumerable<TData> sources, IEnumerable<(Func<TData, string> Selector, Func<TData,bool> Predicate)> transforms)
    {
        _all = sources.Where(src => src.Validate()).ToList();
        foreach (var transform in transforms)
        {
            var map = new DataMap<string,TData>(_all, transform.Selector, transform.Predicate);
            _maps.Add(map);
            foreach (var kvp in map)
                _combined[kvp.Key] = kvp.Value;
        }
    }

    #region IReadOnlyDictionary<string,TData> Implementation

    public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, TData>)this._combined).Keys;
    public IEnumerable<TData> Values => ((IReadOnlyDictionary<string, TData>)this._combined).Values;
    public int Count => ((IReadOnlyCollection<KeyValuePair<string, TData>>)this._combined).Count;
    public TData this[string key] => ((IReadOnlyDictionary<string, TData>)this._combined)[key];
    public bool ContainsKey(string key) => ((IReadOnlyDictionary<string, TData>)this._combined).ContainsKey(key);
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TData value) => ((IReadOnlyDictionary<string, TData>)this._combined).TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, TData>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, TData>>)this._combined).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._combined).GetEnumerator();

    #endregion
}
