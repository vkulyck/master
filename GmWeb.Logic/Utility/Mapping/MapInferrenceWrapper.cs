using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Utility.Mapping;

public class MapInferrenceWrapper<TSource>
{
    private readonly MappingFactory _factory;
    private readonly IEnumerable<TSource> _enumerable;
    private readonly IQueryable<TSource> _queryable;
    public MapInferrenceWrapper(MappingFactory factory, IEnumerable<TSource> source)
    {
        _factory = factory;
        _enumerable = source;
    }
    public MapInferrenceWrapper(MappingFactory factory, IQueryable<TSource> source)
    {
        _factory = factory;
        _queryable = source;
    }
    public IEnumerable<TResult> To<TResult>()
    {
        if(_enumerable != null)
            return _factory.Map<TSource, TResult>(_enumerable);
        if(_queryable != null)
            return _factory.Map<TSource, TResult>(_queryable);
        throw new ArgumentNullException();
    }

    public async IAsyncEnumerable<TResult> ToAsync<TResult>()
    {
        if (_queryable == null)
            throw new ArgumentNullException();
        await foreach (var item in _queryable.AsAsyncEnumerable())
        {
            var mapped = _factory.Map<TSource, TResult>(item);
            yield return mapped;
        }
    }
}