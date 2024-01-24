using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Datasets.Abstract;

public class DataMap<TKey, TData> : Dictionary<TKey,TData>
    where TData : DataItem
{
    private readonly Func<TData, TKey> _selector;
    public TKey GetDataKey(TData data) => _selector(data);

    public DataMap(IEnumerable<TData> sources, Func<TData,TKey> selector, Func<TData, bool> predicate)
    {
        this._selector = selector;
        foreach (var source in sources)
        {
            if (predicate is not null)
                if (!predicate(source))
                    continue;
            var key = selector(source);
            if (key is null)
                continue;
            if (key is string sKey)
                if (string.IsNullOrWhiteSpace(sKey))
                    continue;
            this[key] = source;
        }
    }
}