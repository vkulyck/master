using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using Newtonsoft.Json;
using GmWeb.Logic.Enums;
using SystemIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GmWeb.Logic.Utility.Performance.Paging;
public interface IPagedListRequest
{
    public int PageIndex { get; }
    public int PageSize { get; }
}
public record class PagedListRequest : IPagedListRequest
{
    public virtual int PageIndex { get; set; } = 0;
    public virtual int PageSize { get; set; } = 0;
}
public interface IExtendedPageListRequest<TKey> : IPagedListRequest
{
    public TKey GroupKey { get; }
}
public interface IExtendedPageListRequest<TEntity, TKey> : IExtendedPageListRequest<TKey>
{
    public Expression<Func<TEntity, TKey>> GroupKeySelector { get; }
    public Expression<Func<TEntity, TKey>> SortKeySelector { get; }
    public Expression<Func<TEntity, bool>>[] FilterPredicates { get; }
}
public abstract record class ExtendablePagedListRequest<TEntity, TKey> : PagedListRequest
{
    public abstract IExtendedPageListRequest<TEntity, TKey> Extend();
}
public abstract record class AlphabeticalListRequest<TEntity> : ExtendablePagedListRequest<TEntity, string>
{
    public virtual string Letter { get; set; }
}
public record class ExtendedAlphabeticalListRequest<TEntity> : PagedListRequest, IExtendedPageListRequest<TEntity, string>
{
    public string GroupKey { get; set; }
    [SystemIgnore]
    public Expression<Func<TEntity, string>> GroupKeySelector { get; set; }
    [SystemIgnore]
    public Expression<Func<TEntity, string>> SortKeySelector { get; set; }    
    [SystemIgnore]
    public Expression<Func<TEntity, bool>>[] FilterPredicates { get; set; }
}

public abstract record class TimePeriodListRequest<TEntity> : ExtendablePagedListRequest<TEntity, DateTime>
{
    [SystemIgnore]
    public TimePeriod Period { get; set; }
    public DateTime PageDate { get; set; } = DateTime.Now;
}
public record class ExtendedTimePeriodListRequest<TEntity> : PagedListRequest, IExtendedPageListRequest<TEntity, DateTime>
{
    public DateTime GroupKey { get; set; }
    [SystemIgnore]
    public Expression<Func<TEntity, DateTime>> GroupKeySelector { get; set; }
    [SystemIgnore]
    public Expression<Func<TEntity, DateTime>> SortKeySelector { get; set; }
    [SystemIgnore]
    public Expression<Func<TEntity, bool>>[] FilterPredicates { get; set; }
}
