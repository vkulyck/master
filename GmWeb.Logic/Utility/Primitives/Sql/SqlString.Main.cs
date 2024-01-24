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

/// <summary>
/// A simple wrapper for raw SQL expression strings with support for built-in functions. 
/// The list of supported functions will be expanded as needed.
/// </summary>
/// <remarks>
/// Planned Improvements: Create a t4 template that will generate functions from syntax definitions.
/// For example, the official function specifications list a number of JSON functions
/// along with their syntax definitions. The template should generate a file named
/// 'SqlString.JSON.cs' with a partial class definition for SqlString and 
/// implementations for each JSON function according to its syntax.
/// See <see href="https://docs.microsoft.com/en-us/sql/t-sql/functions/json-functions-transact-sql?view=sql-server-ver15">
/// JSON Function Documentation</see>.
/// </remarks>
public partial record struct SqlString
{
    public string SQL { get; }
    public SqlString(string sql) => this.SQL = sql;

    public static implicit operator string(SqlString sqlString) => sqlString.SQL;
    public static implicit operator SqlString(string sql) => new SqlString(sql);
    public static SqlString Column(string name) => new SqlString($"[{name}]");
    public static SqlString Constant(string Value) => new SqlString(Value);
    public static SqlString Constant<T>(T? Value)
        where T : struct
        => new SqlString(Value?.ToString());

    public override string ToString() => this.SQL;
}
