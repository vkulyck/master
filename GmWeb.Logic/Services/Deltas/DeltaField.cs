using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Utility.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Models.Carma;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

public class DeltaField
{
    public string CurrentValue { get; set; }
    public List<DateTimeOffset> Timestamps { get; set; } = new List<DateTimeOffset>();
    public List<Delta> Deltas { get; set; } = new List<Delta>();
    public bool IsUnmodified => this.CurrentValue == null && this.Timestamps.Count == 0 && this.Deltas.Count == 0;
}

public static class DeltaFieldExtensions
{
    public static PropertyBuilder<DateTimeOffset?> ConfigureDeltaModified<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, DateTimeOffset?>> selector,
        params Expression<Func<TEntity, DateTimeOffset?>>[] fallbacks
    )
        where TEntity : class
    {
        var propertyName = selector.GetPropertyInfo().Name.Replace("Modified","");
        SqlString sql = SqlString.Column($"{propertyName}History")
            .JsonValue("$.Timestamps[0]")
            .Trim()
            .Convert<DateTimeOffset>()
            .Coalesce(fallbacks)
        ;
        return builder.Property(selector).HasComputedColumnSql(sql, false);
    }
    public static PropertyBuilder<TProperty> ConfigureDeltaValue<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> selector
    )
        where TEntity : class
    {
        var propertyName = selector.GetPropertyInfo().Name;
        var sql = SqlString
            .Column($"{propertyName}History")
            .JsonValue("$.CurrentValue")
        ;
        return builder.Property(selector).HasComputedColumnSql(sql, false);
    }
}