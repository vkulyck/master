using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Interfaces
{
    public interface IMultiQueryDataRowModel : IDataRowModel
    {
        void PerformAdditionalQueries(DbContext context);
    }
}
