using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GmWeb.Logic.Interfaces
{
    public interface IDataRowModel
    {
        void LoadDataRow(DataRow row);
        void LoadDataRow<TContext>(DataRow row, TContext context) where TContext : DbContext;
    }
}
