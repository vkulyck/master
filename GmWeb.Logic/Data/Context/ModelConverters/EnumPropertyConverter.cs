using GmWeb.Logic.Utility.Extensions.Dynamic;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class EnumPropertyConverter<TStoreProperty> : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            if (!this.ModelPropertyType.IsEnum)
                return false;
            if (this.DatastoreFieldType == typeof(TStoreProperty))
                return true;
            return false;
        }
        private static Expression<Func<object, object>> EnumSerializer()
        {
            var t = typeof(TStoreProperty);
            if (t == typeof(string))
                return x => x.ToString();
            if (t.IsNumericType())
                return x => t.ConvertNumericType(x);
            return x => x;
        }

        private static Expression<Func<object, object>> EnumDeserializer(Type type)
        {
            if (typeof(TStoreProperty) == typeof(string))
                return x => Enum.Parse(type, x.ToString());
            return x => x;
        }

        public EnumPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : this(property, config, null) { }

        public EnumPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config, Type defaultStoreType) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType,
            DatastoreFieldType: property.ConfiguredStoreType() ?? defaultStoreType,
            ModelToDatastoreConverter: EnumSerializer(),
            DatastoreToModelConverter: EnumDeserializer(property.PropertyType))
        {
        }
    }
}
