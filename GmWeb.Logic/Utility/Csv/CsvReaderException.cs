using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace GmWeb.Logic.Utility.Csv;
public class CsvReaderException : Exception
{
    public string HeaderName { get; set; }
    public string FieldValue { get; set; }
    public int RecordIndex { get; set; }
    public int FieldIndex { get; set; }
    public static string GetErrorMessage(ReadingContext context) =>
        $"'{context.Record[context.CurrentIndex]}' in record {context.RawRow} is not a valid value for {context.HeaderRecord[context.CurrentIndex]}.";
    public CsvReaderException(ReadingContext context, Exception innerException)
        : base(GetErrorMessage(context), innerException)
    {
        this.HeaderName = context.HeaderRecord[context.CurrentIndex];
        this.FieldValue = context.Record[context.CurrentIndex];
        this.RecordIndex = context.RawRow;
        this.FieldIndex = context.CurrentIndex;
    }
}
