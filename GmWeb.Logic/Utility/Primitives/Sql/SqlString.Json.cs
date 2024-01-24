using System;
using System.Collections.Generic;
using SqlDbType = System.Data.SqlDbType;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Primitives;

public partial record struct SqlString
{
    public SqlString IsJson() =>
        this.Wrap("ISJSON");

    public SqlString JsonValue(string path) =>
        this.Wrap("JSON_VALUE", Q(path));
    public SqlString JsonQuery(string path) =>
        this.Wrap("JSON_QUERY", Q(path));
    public SqlString JsonModify<T>(string path, T newValue)
    {
        string nodeValue = newValue switch
        {
            string string_value => string_value,
            _ => newValue.ToString()
        };
        return this.Wrap("JSON_MODIFY", Q(path), nodeValue);
    }
}
