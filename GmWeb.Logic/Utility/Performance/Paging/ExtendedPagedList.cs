using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Performance.Paging;

public class ExtendedPagedList<TEntity, TKey> : PagedList<TEntity>
{
    [JsonIgnore]
    new protected IExtendedPageListRequest<TKey> Request
    {
        get => base.Request as IExtendedPageListRequest<TKey>;
        set => base.Request = value;
    }
    public TKey GroupKey { get; set; }
    public int GroupPageCount => this.PageSize == 0 ? 1 : (int)System.Math.Ceiling(this.GroupItemCount / (double)this.PageSize);
    public int GroupItemCount { get; set; }
    public ExtendedPagedList() { }
    public ExtendedPagedList(IEnumerable<TEntity> items, IExtendedPageListRequest<TKey> request, int totalItemCount, int groupItemCount)
        : base(items, request, totalItemCount)
    {
        Request = request;
        this.GroupItemCount = groupItemCount;
        this.GroupKey = request.GroupKey;
    }

    new public ExtendedPagedList<TDestination, TKey> Map<TDestination>()
    {
        var mappedItems = Mapper.Map<TEntity, TDestination>(this.Items);
        var mappedPL = new ExtendedPagedList<TDestination, TKey>(mappedItems, Request, this.TotalItemCount, this.GroupItemCount);
        return mappedPL;
    }
}
