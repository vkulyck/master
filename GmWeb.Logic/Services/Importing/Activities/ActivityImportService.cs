using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GmWeb.Logic.Services.Importing.Activities.Spreadsheets;
using GmWeb.Logic.Data.Context.Carma;

namespace GmWeb.Logic.Services.Importing.Activities;
public class ActivityImportService : ImportService
{
    private readonly ActivityImportOptions _options;
    private readonly CarmaCache _cache;
    private readonly ILogger<ActivityImportService> _logger;
    private readonly ActivityReader _reader;

    public ActivityImportOptions Options => this._options;
    protected CarmaCache Cache => this._cache;
    public ActivityImportService(IOptions<ActivityImportOptions> options, CarmaContext context, ILoggerFactory factory)
        : this(options?.Value, context, factory)
    { }
    public ActivityImportService(ActivityImportOptions options, CarmaContext context, ILoggerFactory factory)
    {
        this._options = options;
        this._cache = new CarmaCache(context);
        this._logger = factory.CreateLogger<ActivityImportService>();
        this._reader = new ActivityReader(options, context);
    }

    public async override Task RunAsync()
    {
        if (!this.Options.Enabled)
            return;
        var models = await this._reader.ReadModelsAsync().ToListAsync();
        foreach (var model in models)
        {
            _logger.LogDebug($"Creating: {model.Name} {model.StartTime:yyyy-MM-dd} {model.StartTime:HH:mm} -> {model.EndTime:HH:mm}");
            model.AgencyID = this.Options.AgencyID;
            await this.Cache.ActivityCalendars.InsertScheduleAsync(model);
        }
        await this.Cache.SaveAsync();
    }
}
