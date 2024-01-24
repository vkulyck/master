using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Utility.Web.Html;
public class TableRow
{
    public int RowIndex { get; private set; }
    public TableRow(int index)
    {
        this.RowIndex = index;
    }
}
public class TableRow<T> : TableRow where T : struct
{
    public Table<T> Table { get; private set; }
    public IEnumerable<TableCell<T>> Cells => this.Table.Cells.Where(x => x.RowIndex == this.RowIndex);
    public TableRow(Table<T> table, int index) : base(index)
    {
        this.Table = table;
    }
}