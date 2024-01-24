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

namespace GmWeb.Logic.Data.Context.Carma;

public class DeltaField
{
    public string CurrentValue { get; set; }
    public List<DateTimeOffset> Timestamps { get; set; } = new List<DateTimeOffset>();
    public List<Delta> Deltas { get; set; } = new List<Delta>();
}

public static class BitVectorExtensions
{
    public static PropertyBuilder<TProperty> ConfigureBitValueColumn<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> selector
    ) where TEntity : class
    => builder.ConfigureBitValueColumn(selector, mask: 0x01);

    public static PropertyBuilder<TProperty> ConfigureBitValueColumn<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TProperty>> selector,
        int mask
    ) where TEntity : class
    {
        var sql = SqlString
            .Column("Status")
            .HasFlag(mask)
            .Convert<bool>()
        ;
        return builder.Property(selector).HasComputedColumnSql(sql, false);
    }
}