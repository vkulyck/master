using Newtonsoft.Json;

namespace GmWeb.Logic.Services.Importing.Clients;
public class ClientImportOptions : ImportOptions
{
    public int AgencyID { get; set; }
    public int BatchSize { get; set; } = 50;
    public int? MaximumWidth { get; set; } = null;
    public int? MaximumHeight { get; set; } = null;
    public bool EnableCompression { get; set; } = false;
    public bool GenerateMissingData { get; set; } = false;
    public int StartingUserID { get; set; }
    public string SourceImageDirectory { get; set; }
    public string ProcessedImageDirectory { get; set; }
    public string SourceClientSpreadsheet { get; set; }
    public ImportSources ImportSources { get; set; } = ImportSources.None;
    public string DefaultClientEmailDomain { get; set; }
}