using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Services.Deltas;
using Countries.NET;
using Countries.NET.Enums;

namespace GmWeb.Logic.Services.Datasets.Abstract;

public abstract class DatasetMatchingOptions : ServiceOptions, IExportOptions
{
    public string OutputDirectory { get; set; }
    public DeltaOptions Deltas { get; set; } = new DeltaOptions();
    public List<string> Queries { get; set; }
}
