using GmWeb.Logic.Data.Annotations;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class FixedStringPropertyConverter : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            if (this.ModelProperty.HasEncryptedDatastore())
                return false;
            bool configurable = this.ModelProperty.PropertyType == typeof(string);
            return configurable;
        }
        private static Expression<Func<object, object>> GetSerializer() => x => x;
        private static Expression<Func<object, object>> GetDeserializer() => x => x == null ? null : x.ToString().Trim();

        public FixedStringPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: typeof(string),
            DatastoreFieldType: typeof(string),
            GetSerializer(), GetDeserializer()
        )
        { }
    }
}
