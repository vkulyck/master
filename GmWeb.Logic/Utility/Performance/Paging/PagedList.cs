using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Performance.Paging;

public class PagedList<T>
{
    protected IPagedListRequest Request { get; set; }
    public static MappingFactory Mapper => MappingFactory.Instance;
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 0;
    public int TotalPageCount => this.PageSize == 0 ? 1 : (int)System.Math.Ceiling(this.TotalItemCount / (double)this.PageSize);
    public int TotalItemCount { get; set; }
    public List<T> Items { get; } = new List<T>();
    public PagedList() { }
    public PagedList(IEnumerable<T> items, IPagedListRequest request, int totalItemCount)
    {
        this.TotalItemCount = totalItemCount;
        this.PageSize = request.PageSize;
        this.PageIndex = request.PageIndex;
        Request = request;
        this.Items.AddRange(items);
    }

    public PagedList<TDestination> Map<TDestination>()
    {
        var mappedItems = Mapper.Map<T, TDestination>(this.Items);
        var mappedPL = new PagedList<TDestination>(mappedItems, Request, this.TotalItemCount);
        return mappedPL;
    }
}