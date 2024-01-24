using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Utility.Web.Html;
public abstract class TableCell
{
    public Table Table { get; private set; }
    public IReadOnlyList<string> Headers { get; private set; }
    public IReadOnlyList<string> Keys { get; private set; }
    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }

    protected TableCell(Table table, int row, int column)
    {
        this.Keys = table.Keys[row].ToList();
        this.Headers = table.Headers[column].ToList();
        this.RowIndex = row;
        this.ColumnIndex = column;
        this.Table = table;
    }

    public TableCell<T> Cast<T>() where T : struct => this as TableCell<T>;
}

public class TableCell<T> : TableCell where T : struct
{
    public T Data { get; private set; }
    public new Table<T> Table => base.Table.Cast<T>();
    public TableCell(Table<T> table, int row, int column, T data) : base(table, row, column)
    {
        this.Data = data;
    }
}