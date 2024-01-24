using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Csv;
using GmWeb.Logic.Data.Context.Carma;
using ActivityModel = GmWeb.Logic.Data.Models.Carma.Activity;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;
using CsvHelper.TypeConversion;

namespace GmWeb.Logic.Services.Importing.Activities.Spreadsheets;
public class ActivityReader : CsvReader<ActivityRecord>
{
    public string Name => "Activity-Spreadsheet";
    protected MappingFactory Mapper => MappingFactory.Instance;
    public ActivityImportOptions Options { get; }
    protected CarmaContext Context { get; }
    public ActivityReader(ActivityImportOptions options, CarmaContext context)
        : base(options.SourceActivitySpreadsheet, HeaderCount: 2)
    {
        this.Options = options;
        this.Context = context;
        var tcOpts = new TypeConverterOptions();
        tcOpts.NullValues.AddRange(options.NullValueDelimiters);
        this.WrappedCsvReader.Configuration.TypeConverterOptionsCache.AddOptions<DateTime?>(tcOpts);
        this.WrappedCsvReader.Configuration.TypeConverterOptionsCache.AddOptions<int?>(tcOpts);
    }

    public IAsyncEnumerable<ActivityModel> ReadModelsAsync() =>
        this.GetRecordsAsync().SelectAwait(async x => 
            await x.LoadModelAsync(this.Options, this.Context)
        );
}
