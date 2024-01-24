using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GmWeb.Logic.Utility.Web.Html;
public class Table
{
    public IReadOnlyList<IReadOnlyList<string>> Keys { get; protected set; }
    public IReadOnlyList<IReadOnlyList<string>> Headers { get; protected set; }

    protected Table() { }

    protected static List<List<string>> extractHeaders(HtmlNode node, int keyCount)
    {
        var headers = new List<List<string>>();
        var theads = node.Descendants("thead").ToList();
        for (int i = 0; i < theads.Count; i++)
        {
            var headerLine = new List<string>();
            var ths = theads[i].Descendants("th").ToList();
            if (ths.Count == 0)
                ths = theads[i].Descendants("td").ToList();
            for (int j = keyCount; j < ths.Count; j++)
            {
                string colspan = ths[j].Attributes["colspan"]?.Value;
                if (!int.TryParse(colspan, out int ispan))
                    ispan = 1;
                for (int k = 0; k < ispan; k++)
                {
                    string header = ths[j].InnerText.Trim();
                    header = HtmlEntity.DeEntitize(header);
                    headerLine.Add(header);
                }
            }
            headers.Add(headerLine);
        }
        headers = headers.Zip();
        return headers;
    }

    protected static List<List<string>> extractKeys(HtmlNode node, int keyCount)
    {
        // fill each column of key values at a time iterating over rowspans as needed
        var keys = new List<List<string>>();
        var trs = node.Element("tbody").Elements("tr").ToList();
        for (int i = 0; i < keyCount; i++)
        {
            var keyColumn = new List<string>();
            // Iterate over all rows to get the first key in each
            for (int j = 0; j < trs.Count; j++)
            {
                // Get the first cell for this row and its rowspan
                var td = trs[j].Descendants("td").Skip(i).First();
                string rowspan = trs[j].Attributes["rowspan"]?.Value;
                if (!int.TryParse(rowspan, out int ispan))
                    ispan = 1;
                for (int k = 0; k < ispan; k++)
                {
                    string key = td.InnerText.Trim();
                    key = HtmlEntity.DeEntitize(key);
                    keyColumn.Add(key);
                }
            }
            keys.Add(keyColumn);
        }
        keys = keys.Zip();
        return keys;
    }

    public Table<T> Cast<T>() where T : struct => (Table<T>)this;

    public static Table<T> FromNode<T>(HtmlNode node, int keyCount = 1) where T : struct => Table<T>.FromNode(node, keyCount);
}
public class Table<T> : Table where T : struct
{
    protected Dictionary<(int, int), TableCell<T>> _Cells { get; } = new Dictionary<(int, int), TableCell<T>>();
    public IReadOnlyList<TableRow<T>> Rows { get; protected set; }
    public IReadOnlyList<TableColumn<T>> Columns { get; private set; }
    public IEnumerable<TableCell<T>> Cells => this._Cells.Values.AsEnumerable();

    public TableCell<T> this[int Row, int Column]
    {
        get => this._Cells[(Row, Column)];
        set => this._Cells[(Row, Column)] = value;
    }

    private Table() { }

    public static Table<T> FromNode(HtmlNode node, int keyCount = 1)
    {
        var table = new Table<T>
        {
            Headers = extractHeaders(node, keyCount),
            Keys = extractKeys(node, keyCount)
        };

        var trs = node.Element("tbody").Elements("tr").ToList();
        for (int i = 0; i < trs.Count; i++)
        {
            var tds = trs[i].Elements("td").ToList();
            for (int j = keyCount; j < tds.Count; j++)
            {
                string inner = tds[j].InnerText;
                var style = System.Globalization.NumberStyles.Currency;
                var provider = System.Globalization.CultureInfo.CurrentCulture;
                if (inner.TryParse<T>(style, provider, out var result))
                {
                    int row = i;
                    int column = j - keyCount;
                    var cell = new TableCell<T>(table, row, column, result);
                    table[i, j] = cell;
                }
            }
        }
        table.Rows = Enumerable.Range(0, table.Keys.Count).Select(i => new TableRow<T>(table, i)).ToList();
        table.Columns = Enumerable.Range(0, table.Headers.Count).Select(i => new TableColumn<T>(table, i)).ToList();
        return table;
    }
}