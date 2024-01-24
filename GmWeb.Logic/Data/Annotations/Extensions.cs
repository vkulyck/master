using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions.Reflection;

namespace GmWeb.Logic.Data.Annotations
{
    public static class Extensions
    {
        public static bool HasEncryptedDatastore(this PropertyInfo property)
        {
            bool result = property.GetCustomAttributes(true).OfType<IEncryptedColumnAttribute>().Any();
            return result; ;
        }
    }
}
