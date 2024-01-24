using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Utility.Web.Html;
public class TableColumn
{
    public int ColumnIndex { get; private set; }
    public TableColumn(int index)
    {
        this.ColumnIndex = index;
    }
}
public class TableColumn<T> : TableColumn where T : struct
{
    public Table<T> Table { get; private set; }
    public IEnumerable<TableCell<T>> Cells => this.Table.Cells.Where(x => x.ColumnIndex == this.ColumnIndex);
    public TableColumn(Table<T> table, int index) : base(index)
    {
        this.Table = table;
    }
}