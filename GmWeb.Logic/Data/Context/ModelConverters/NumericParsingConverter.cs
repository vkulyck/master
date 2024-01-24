using GmWeb.Logic.Utility.Extensions.Dynamic;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class NumericParsingConverter : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            if (!this.ModelPropertyType.IsNumericType())
                return false;
            if (this.DatastoreFieldType == typeof(string))
                return true;
            return false;
        }
        public static Expression<Func<object, object>> NumericSerializer() => (object x) => x.ToString();

        public static Expression<Func<object, object>> NumericDeserializer(Type propertyType)
        {
            var deserializer = propertyType.NumericStringConverterExpression(-1);
            return deserializer;
        }

        public NumericParsingConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : this(property, config, null) { }

        public NumericParsingConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config, Type defaultStoreType) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType,
            DatastoreFieldType: property.ConfiguredStoreType() ?? defaultStoreType,
            ModelToDatastoreConverter: NumericSerializer(),
            DatastoreToModelConverter: NumericDeserializer(property.PropertyType)
        )
        {
        }
    }
}
