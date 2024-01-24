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
    public SqlString Coalesce<TEntity, TProperty>(IEnumerable<Expression<Func<TEntity, TProperty>>> fallbacks)
    {
        if (fallbacks == null || fallbacks.Count() == 0)
            return this;
        return this.Wrap(nameof(Coalesce), fallbacks.Select(x => $"[{x.GetPropertyInfo().Name}]"));
    }
}
