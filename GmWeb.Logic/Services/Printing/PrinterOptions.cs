using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Printing;

public class PrinterOptions : ServiceOptions
{
    public bool EnablePrinting { get; set; } = false;
    public bool EnableExport { get; set; } = false;
    public string CommandExportDirectory { get; set; }
    public string PrinterHost { get; set; } = "localhost";
    public int PrinterPort { get; set; }
    public TimeSpan ResponseTimeout { get; set; } = TimeSpan.Zero;
    public int? UseRecordStartIndex { get; set; } = 0;
    public int? UseRecordCount { get; set; }
    public bool ClientsOnly { get; set; } = false;
}
