using GmWeb.Logic.Data.Context.ModelConverters;
using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConfigurators
{
    public class PropertySerializationConfiguration<TEntity> : IDynamicConverterConfiguration<TEntity> 
        where TEntity : class
    {
        public List<Type> ConverterTypes { get; } = new List<Type>();
        public IConfiguration Config { get; private set; }

        public PropertySerializationConfiguration(IConfiguration config)
        {
            this.Config = config;
        }
        public IDynamicConverterConfiguration<TEntity> With<TConverter>()
        {
            this.ConverterTypes.Add(typeof(TConverter));
            return this;
        }

        private bool TrySetConversion(EntityTypeBuilder<TEntity> builder, PropertyInfo property, Type converterType)
        {
            var converter = Activator.CreateInstance(converterType, property, this.Config) as DynamicValueConverter;
            if (converter == null)
                return false;
            if (!converter.CanConfigureProperty())
                return false;
            builder.Property(property.PropertyType, property.Name).HasConversion(converter);
            return true;
        }

        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                if (!property.IsMapped())
                    continue;
                if (property.SetMethod == null)
                    continue;
                var cfgStoreType = property.ConfiguredStoreType();
                bool isStoreNumeric = cfgStoreType?.IsNumericType() ?? false;
                if (isStoreNumeric && property.PropertyType.IsEnum)
                {
                    var enumConverter = typeof(EnumPropertyConverter<>).MakeGenericType(cfgStoreType);
                    if (this.TrySetConversion(builder, property, enumConverter))
                        continue;
                }
                var types = this.ConverterTypes.ToList();
                types.Reverse();
                foreach (var type in types)
                    if (!this.TrySetConversion(builder, property, type))
                        continue;
            }
        }
    }
}
