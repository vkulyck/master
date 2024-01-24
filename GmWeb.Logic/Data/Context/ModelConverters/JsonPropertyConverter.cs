using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Utility.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class JsonPropertyConverter : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            var attribute = this.ModelProperty.GetCustomAttributes(true).OfType<JsonColumnAttribute>().SingleOrDefault();
            return attribute != null;
        }
        private static Expression<Func<object, object>> JsonSerializer() => x => JsonConvert.SerializeObject(x, SerializationExtensions.JsonSerializerSettings);

        private static Expression<Func<object, object>> JsonDeserializer(Type type) => x => JsonConvert.DeserializeObject(x == null ? string.Empty : x.ToString(), type, SerializationExtensions.JsonSerializerSettings);

        public JsonPropertyConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType, DatastoreFieldType: typeof(string),
            ModelToDatastoreConverter: JsonSerializer(),
            DatastoreToModelConverter: JsonDeserializer(property.PropertyType))
        {
        }
    }
}
