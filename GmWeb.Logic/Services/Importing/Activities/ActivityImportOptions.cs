using System.Collections.Generic;

namespace GmWeb.Logic.Services.Importing.Activities;
public class ActivityImportOptions : ImportOptions
{
    public int AgencyID { get; set; }
    public int BatchSize { get; set; } = 50;
    public string SourceActivitySpreadsheet { get; set; }
    public List<string> NullValueDelimiters { get; set; }
}