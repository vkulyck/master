using GmWeb.Common.Crypto;
using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Utility.Config;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class EncryptedDateTimeConverter : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            var attribute = this.ModelProperty.GetCustomAttributes(true).OfType<EncryptedDateTimeColumnAttribute>().SingleOrDefault();
            bool result = attribute != null;
            return result;
        }
        public static Expression<Func<object, object>> DateTimeEncryptor(DatabaseConnectionOptions settings)
        {
            var serializer = DateTimePropertyConverter<string>.DateTimeSerializer().Compile();
            return x => SymmetricEncryptor.EncryptObject(serializer(x), settings.EncryptionKey);
        }

        public static Expression<Func<object, object>> DateTimeDecryptor(DatabaseConnectionOptions settings)
        {
            var deserializer = DateTimePropertyConverter<string>.DateTimeDeserializer().Compile();
            return x => deserializer(SymmetricEncryptor.DecryptObject(x, settings.EncryptionKey));
        }

        public EncryptedDateTimeConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType, DatastoreFieldType: typeof(string),
            ModelToDatastoreConverter: DateTimeEncryptor(config.Db()),
            DatastoreToModelConverter: DateTimeDecryptor(config.Db()))
        {
        }
    }
}
