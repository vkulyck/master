using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public abstract class DynamicValueConverter : ValueConverter
    {
        protected IConfiguration Config { get; private set; }
        public Type StoreType { get; protected set; } = typeof(string);
        public virtual bool CanConfigureProperty() => false;

        protected Expression<Func<object, object>> Serializer { get; private set; }
        protected Func<object, object> ConvertToProviderFunc = null;
        public override Func<object, object> ConvertToProvider => this.ConvertToProviderFunc ?? (this.ConvertToProviderFunc = this.Serializer.Compile());

        protected Expression<Func<object, object>> Deserializer { get; private set; }
        protected Func<object, object> ConvertFromProviderFunc = null;
        public override Func<object, object> ConvertFromProvider => this.ConvertFromProviderFunc ?? (this.ConvertFromProviderFunc = this.Deserializer.Compile());

        protected Type ModelPropertyType { get; private set; }
        public override Type ModelClrType => this.ModelPropertyType;

        protected Type DatastoreFieldType { get; private set; }
        public override Type ProviderClrType => this.DatastoreFieldType;

        public PropertyInfo ModelProperty { get; private set; }

        public DynamicValueConverter(
            PropertyInfo ModelProperty = null,
            IConfiguration Config = null,
            Type ModelPropertyType = null, Type DatastoreFieldType = null,
            Expression<Func<object, object>> ModelToDatastoreConverter = null,
            Expression<Func<object, object>> DatastoreToModelConverter = null
        ) : base(ModelToDatastoreConverter, DatastoreToModelConverter)
        {
            this.ModelProperty = ModelProperty ?? throw new ArgumentNullException("ModelProperty");
            this.ModelPropertyType = ModelPropertyType ?? throw new ArgumentNullException("ModelPropertyType");
            this.DatastoreFieldType = DatastoreFieldType;
            this.Serializer = ModelToDatastoreConverter;
            this.Deserializer = DatastoreToModelConverter;
            this.Config = Config;
        }
    }
}
