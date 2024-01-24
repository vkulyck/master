using System;
using System.Collections.Generic;
using SqlDbType = System.Data.SqlDbType;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Logic.Utility.Extensions.Dynamic;

namespace GmWeb.Logic.Utility.Primitives;

public partial record struct SqlString
{
    public SqlString Trim() => $"TRIM({this.SQL})";
    public SqlString Convert<T>()
    {
        var sqlType = typeof(T).ToSqlType()?.ToString().ToUpper();
        if (sqlType == null)
            throw new NotImplementedException();
        SqlString converted = $"CONVERT({sqlType}, {this.SQL})";
        return converted;
    }
}
