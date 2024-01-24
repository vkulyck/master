using GmWeb.Common.Crypto;
using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Utility.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class EncryptedStringConverter : DynamicValueConverter
    {
        public override bool CanConfigureProperty()
        {
            var attribute = this.ModelProperty.GetCustomAttributes(true).OfType<EncryptedStringColumnAttribute>().SingleOrDefault();
            bool result = attribute != null;
            return result;
        }
        public static object EncryptString(object str, DatabaseConnectionOptions settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));
            var result = SymmetricEncryptor.EncryptObject(str, settings.EncryptionKey);
            return result;
        }
        public static object DecryptString(object str, DatabaseConnectionOptions settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));
            var result = SymmetricEncryptor.DecryptObject(str, settings.EncryptionKey);
            return result;
        }
        public static Expression<Func<object, object>> StringEncryptor(DatabaseConnectionOptions settings) => x => EncryptString(x, settings);

        public static Expression<Func<object, object>> StringDecryptor(DatabaseConnectionOptions settings) => x => DecryptString(x, settings);

        public EncryptedStringConverter(PropertyInfo property, IConfiguration config) : base(
            ModelProperty: property, Config: config,
            ModelPropertyType: property.PropertyType, DatastoreFieldType: typeof(string),
            ModelToDatastoreConverter: StringEncryptor(config.Db()),
            DatastoreToModelConverter: StringDecryptor(config.Db()))
        {
        }
    }
}
