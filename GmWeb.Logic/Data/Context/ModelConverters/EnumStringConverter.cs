using System.Reflection;

namespace GmWeb.Logic.Data.Context.ModelConverters
{
    public class EnumStringConverter : EnumPropertyConverter<string>
    {
        public EnumStringConverter(PropertyInfo property, Microsoft.Extensions.Configuration.IConfiguration config)
            : base(property, config, typeof(string))
        { }
    }
}
