using GmWeb.Logic.Data.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class DateTimeOffsetPropertyConverter<TStoreProperty> : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            if (this.ModelProperty.HasEncryptedDatastore())
                return false;
            if (!this.ModelPropertyType.IsAssignableFrom(typeof(DateTimeOffset)))
                return false;
            if (this.DatastoreFieldType == typeof(TStoreProperty))
                return true;
            return false;
        }
        public static Expression<Func<object, object>> DateTimeSerializer()
        {
            return (object x) =>
                x == null ? null
                : (x as DateTimeOffset?).Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
            ;
        }

        public static Expression<Func<object, object>> DateTimeDeserializer()
        {
            var formats = new List<string>
            {
                @"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz",
                "MMM d yyyy hh:mm:ss:ffftt",
                "MMM d yyyy hh:mmtt", // Apr 12 1920 12:00AM
                "yyyy-MM-ddThh:mm:ss",
                "o"
            };
            var prv = CultureInfo.InvariantCulture;
            Func<object, string> sanitize = x => Regex.Replace(x.ToString(), " +", " ");
            Func<object, DateTimeOffset> parse = x => DateTimeOffset.ParseExact(sanitize(x), formats.ToArray(), prv, DateTimeStyles.None);
            Expression<Func<object, object>> deserialize = (object x) => x == null ? (DateTimeOffset?)null : parse(x);
            return deserialize;
        }

        public DateTimeOffsetPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : this(property, config, null) { }

        public DateTimeOffsetPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config, Type defaultStoreType) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType,
            DatastoreFieldType: property.ConfiguredStoreType() ?? defaultStoreType,
            ModelToDatastoreConverter: DateTimeSerializer(),
            DatastoreToModelConverter: DateTimeDeserializer()
        )
        {
        }
    }
}
