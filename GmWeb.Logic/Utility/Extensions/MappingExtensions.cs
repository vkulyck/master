using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Mapping;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;

namespace GmWeb.Logic.Utility.Extensions.Mapping;
public static class MappingExtensions
{
    public static IEnumerable<TDest> Map<TSource,TDest>(this IQueryable<TSource> items, MappingFactory mapper)
    {
        var mapped = mapper.Map<TSource,TDest>(items);
        return mapped;
    }
}
