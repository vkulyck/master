using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Data.Context.ModelConfigurators;
using GmWeb.Logic.Data.Context.ModelConverters;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GmWeb.Logic.Data.Context
{
    public static class BuilderExtensions
    {
        public static ModelBuilder WithDefaultConversions<TEntity>(this ModelBuilder builder, IConfiguration config) where TEntity : class
        {
            var configurations = new List<IDynamicConverterConfiguration<TEntity>>
            {
                new PropertySerializationConfiguration<TEntity>(config)
            };
            foreach (var cfg in configurations)
            {
                cfg
                   .With<EnumStringConverter>()
                   .With<JsonPropertyConverter>()
                   .With<FixedStringPropertyConverter>()
                   .With<DateTimePropertyConverter<string>>()
                   .With<DateTimeOffsetPropertyConverter<string>>()
                   .With<NumericParsingConverter>()
                   .With<EncryptedStringConverter>()
                   .With<EncryptedDateTimeConverter>()
                ;
                builder.ApplyConfiguration(cfg);
            }
            return builder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureGeneratedEntity<TEntity>(this ModelBuilder builder) where TEntity : class => throw new NotImplementedException();
        public static EntityTypeBuilder<TEntity> ConfigureGeneratedEntity<TEntity>(this ModelBuilder builder, IConfiguration config) where TEntity : class => builder.WithDefaultConversions<TEntity>(config).Entity<TEntity>();
        public static EntityTypeBuilder<TEntity> ConfigureEntity<TEntity>(this ModelBuilder builder, IConfiguration config, string TableName) where TEntity : class => builder.WithDefaultConversions<TEntity>(config).Entity<TEntity>().ToTable(TableName);

        public static EntityTypeBuilder<TEntity> ConfigureEntity<TEntity>(this ModelBuilder builder, IConfiguration config, string TableName, string Schema) where TEntity : class => builder.WithDefaultConversions<TEntity>(config).Entity<TEntity>().ToTable(TableName, Schema);

        public static Type ConfiguredStoreType(this PropertyInfo property)
        {
            var columnAttributes = property.GetCustomAttributes<SqlDataTypeAttribute>().ToList();
            var attr = columnAttributes.SingleOrDefault();
            if (attr is null)
                return default;
            var storeClrType = attr.StoreType.ToClrType();
            return storeClrType;
        }
    }
}
