using GmWeb.Logic.Utility.Performance.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Web.Api.Utility
{
    public class PagedListResult<T> : ObjectResult
    {
        public new PagedList<T> Value
        {
            get => (PagedList<T>)base.Value;
            set => base.Value = value;
        }
        public PagedListResult(PagedList<T> value) : base(value) { }
        public PagedListResult(IEnumerable<T> source, IPagedListRequest request)
            : this(source.Page(request)) { }
        public PagedListResult<TDestination> Map<TDestination>()
            => new(this.Value.Map<TDestination>());
    }

    public class ExtendedPagedListResult<TEntity,TKey> : ObjectResult
    {
        public new ExtendedPagedList<TEntity,TKey> Value
        {
            get => (ExtendedPagedList<TEntity, TKey>)base.Value;
            set => base.Value = value;
        }
        public ExtendedPagedListResult(ExtendedPagedList<TEntity,TKey> value) : base(value) { }
        public ExtendedPagedListResult<TDest,TKey> Map<TDest>()
            => new(this.Value.Map<TDest>());
    }

    public class ExtendedPagedListCollectionResult<TEntity, TKey> : ObjectResult
    {
        public new List<ExtendedPagedList<TEntity, TKey>> Value
        {
            get => (List<ExtendedPagedList<TEntity, TKey>>)base.Value;
            set => base.Value = value;
        }
        public ExtendedPagedListCollectionResult(IEnumerable<ExtendedPagedList<TEntity, TKey>> value) : base(value.ToList()) { }
        public ExtendedPagedListCollectionResult<TDest, TKey> Map<TDest>()
        {
            var mappedList = new List<ExtendedPagedList<TDest, TKey>>();
            foreach (var epl in this.Value)
            {
                var mapped = epl.Map<TDest>();
                mappedList.Add(mapped);
            }
            return new ExtendedPagedListCollectionResult<TDest, TKey>(mappedList);
        }
    }

    public static class PagedListExtensions
    {
        public static async Task<PagedListResult<TDestination>> MapAsync<T, TDestination>(this Task<PagedListResult<T>> task)
        {
            var list = await task;
            var mapped = list.Map<TDestination>();
            return mapped;
        }
        public static async Task<ExtendedPagedListResult<TDestination, TKey>> MapAsync<T, TDestination, TKey>(this Task<ExtendedPagedListResult<T, TKey>> task)
        {
            var list = await task;
            var mapped = list.Map<TDestination>();
            return mapped;
        }
        public static PagedListResult<T> ToPagedResult<T>(this IEnumerable<T> source, IPagedListRequest request)
            => new(source, request);
        public static async Task<PagedListResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, IPagedListRequest request)
        {
            var list = await source.PageAsync(request);
            var paged = new PagedListResult<T>(list);
            return paged;
        }
        public static PagedListResult<T> ToResult<T>(this PagedList<T> pagedList)
            => new PagedListResult<T>(pagedList);

        public static async Task<ExtendedPagedListResult<TEntity, TKey>> ToPagedResultAsync<TEntity, TKey>(
            this IQueryable<TEntity> source, IExtendedPageListRequest<TEntity, TKey> request)
        {
            var list = await source.PageAsync(request);
            var paged = new ExtendedPagedListResult<TEntity, TKey>(list);
            return paged;
        }

        public static ExtendedPagedListResult<TEntity, TKey> ToResult<TEntity, TKey>(this ExtendedPagedList<TEntity, TKey> pagedList)
            => new ExtendedPagedListResult<TEntity, TKey>(pagedList);

        public static ExtendedPagedListCollectionResult<TEntity, TKey> ToResult<TEntity, TKey>(this IEnumerable<ExtendedPagedList<TEntity, TKey>> pagedList)
    => new ExtendedPagedListCollectionResult<TEntity, TKey>(pagedList);
    }
}
