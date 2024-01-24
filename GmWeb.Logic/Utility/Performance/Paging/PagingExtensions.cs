using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace GmWeb.Logic.Utility.Performance.Paging;

public static class PagingExtensions
{
    public static PagedList<T> Page<T>(this IEnumerable<T> source)
        => source.Page(new PagedListRequest());
    public static PagedList<T> Page<T>(this IEnumerable<T> source, IPagedListRequest request)
    {
        int totalItemCount = source.Count();
        var processed = source;
        if (request.PageSize > 0)
            processed = processed
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
            ;
        var items = processed.ToList();
        return new PagedList<T>(items, request, totalItemCount);
    }

    public static ExtendedPagedList<TEntity,TKey> Page<TEntity,TKey>(this IEnumerable<TEntity> source, IExtendedPageListRequest<TEntity,TKey> request)
    {
        var queryable = source.ToAsyncEnumerable().AsAsyncQueryable();
        var paged = AsyncContext.Run(async () => await queryable.PageAsync(request));
        return paged;
    }

    public static Task<PagedList<T>> PageAsync<T>(this IQueryable<T> source)
        => source.PageAsync(new PagedListRequest());
    public static async Task<PagedList<T>> PageAsync<T>(this IQueryable<T> source, IPagedListRequest request)
    {
        int totalItemCount = await source.CountAsync();
        var processed = source;
        if (request.PageSize > 0)
            processed = processed
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
            ;
        var items = await processed
            .ToListAsync()
        ;
        return new PagedList<T>(items, request, totalItemCount);
    }

    public static async Task<ExtendedPagedList<TEntity, TKey>> PageAsync<TEntity, TKey>(
        this IQueryable<TEntity> source,
        IExtendedPageListRequest<TEntity, TKey> request
    )
    {
        return await source.AsAsyncEnumerable().AsAsyncQueryable().PageAsync(request);
    }
    public static async Task<ExtendedPagedList<TEntity, TKey>> PageAsync<TEntity, TKey>(
        this IAsyncQueryable<TEntity> source,
        IExtendedPageListRequest<TEntity, TKey> request
    )
    {
        var processed = source;
        if(request.FilterPredicates != null && request.FilterPredicates.Length > 0)
        {
            foreach(var filter in request.FilterPredicates)
            {
                processed = processed.Where(filter);
            }
        }
        int totalItemCount = await processed.CountAsync();
        int groupedItemCount = totalItemCount;
        if (request.GroupKeySelector != null && request.GroupKey != null)
        {
            var expKeyValue = Expression.Constant(request.GroupKey);
            var expPredicate = Expression.Equal(request.GroupKeySelector.Body, expKeyValue);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expPredicate, request.GroupKeySelector.Parameters);
            processed = processed.Where(lambda);
            groupedItemCount = await processed.CountAsync();
        }
        if (request.SortKeySelector != null)
            processed = processed.OrderBy(request.SortKeySelector);
        int groupedPageCount = (int)System.Math.Ceiling(groupedItemCount / (double)request.PageSize);

        // Allow reverse-indexing
        int index = request.PageIndex;
        if (index < 0)
            index += groupedPageCount;

        if(request.PageSize > 0)
            processed = processed
                .Skip(index * request.PageSize)
                .Take(request.PageSize)
            ;
        var items = await processed.ToListAsync();
        return new ExtendedPagedList<TEntity, TKey>(items, request, totalItemCount, groupedItemCount);
    }
    public static async Task<PagedList<TDestination>> MapAsync<TSource, TDestination>(this Task<PagedList<TSource>> task)
    {
        var list = await task;
        var mapped = list.Map<TDestination>();
        return mapped;
    }
}