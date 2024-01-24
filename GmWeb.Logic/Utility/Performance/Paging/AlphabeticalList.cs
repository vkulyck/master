using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Performance.Paging;

public class AlphabeticalList<TEntity> : ExtendedPagedList<TEntity, string>
{
    public AlphabeticalList() { }
    public AlphabeticalList(IEnumerable<TEntity> items, IExtendedPageListRequest<string> request, int totalItemCount, int groupItemCount)
        : base(items, request, totalItemCount, groupItemCount)
    { }

    new public AlphabeticalList<TDest> Map<TDest>()
    {
        var mappedItems = Mapper.Map<TEntity, TDest>(this.Items);
        var mappedPL = new AlphabeticalList<TDest>(mappedItems, Request, this.TotalItemCount, this.GroupItemCount);
        return mappedPL;
    }
}
