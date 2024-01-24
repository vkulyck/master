using System;
using System.Data;

namespace GmWeb.Logic.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SqlDataTypeAttribute : Attribute
    {
        public SqlDbType StoreType { get; private set; }
        public SqlDataTypeAttribute(SqlDbType StoreType)
        {
            this.StoreType = StoreType;
        }
    }
}
