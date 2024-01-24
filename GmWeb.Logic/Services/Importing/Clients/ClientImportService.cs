using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Extensions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BulkConfig = EFCore.BulkExtensions.BulkConfig;
using UserModel = GmWeb.Logic.Data.Models.Carma.User;
using GmWeb.Logic.Services.Importing.Clients.Spreadsheets;
using GmWeb.Logic.Services.Importing.Clients.Imagesets;

namespace GmWeb.Logic.Services.Importing.Clients;
public class ClientImportService : ImportService
{
    private readonly ClientImportOptions _settings;
    private readonly CarmaCache _cache;
    private readonly ILogger<ClientImportService> _logger;
    private readonly ClientSpreadsheet _spreadsheet;
    private readonly ClientImageset _imageset;

    public ClientImportOptions Settings => this._settings;
    public ClientImportService(IOptions<ClientImportOptions> options, CarmaContext context, ILogger<ClientImportService> logger)
        : this(options?.Value, context, logger)
    { }
    public ClientImportService(ClientImportOptions settings, CarmaContext context, ILogger<ClientImportService> logger)
    {
        this._settings = settings;
        this._cache = new CarmaCache(context);
        this._logger = logger;
        if (!string.IsNullOrWhiteSpace(settings.SourceClientSpreadsheet))
            this._spreadsheet = new ClientSpreadsheet(settings, context);
        if (!string.IsNullOrWhiteSpace(settings.SourceImageDirectory))
            this._imageset = new ClientImageset(settings, context);
    }

    public async override Task RunAsync()
    {
        if (!this._settings.Enabled)
            return;
        var batches = new List<List<UserModel>>();
        var batchEnumerator = this.LoadSourceModelBatchesAsync(this._settings.BatchSize).GetAsyncEnumerator();
        var loadTask = batchEnumerator.MoveNextAsync().AsTask();
        await loadTask;
        while (loadTask.Result)
        {
            var batch = batchEnumerator.Current.ToList();
            var processTask = this.ProcessModelBatch(batch);
            loadTask = batchEnumerator.MoveNextAsync().AsTask();
            await Task.WhenAll(processTask, loadTask);

            batches.Add(batch);
            this._logger.LogInformation($"Imported {batch.Count} clients from batch {batches.Count:N0} with ID range: ({batch.First().UserID}..{batch.Last().UserID})");
        }
        this._logger.LogInformation($"Imported {batches.Count} batches of {this._settings.BatchSize} clients each, for a total of {batches.Sum(x => x.Count)} new users.");
        this._logger.LogInformation($"Client ID range: {batches.SelectMany(x => x).Min(x => x.UserID)} to {batches.SelectMany(x => x).Max(x => x.UserID)}");
    }

    protected async Task ProcessModelBatch(List<UserModel> batch)
    {
        this._logger.LogInformation($"Inserting batch of {batch.Count} clients...");
        var cache = new CarmaCache();
        int? timeout = cache.DataContext.Database.GetCommandTimeout();
        await cache.BulkUpdateAsync(batch, new BulkConfig
        {
            BulkCopyTimeout = timeout
        });
        this._logger.LogInformation($"Insert complete; client ID range: ({batch.First().UserID}..{batch.Last().UserID})");
    }

    protected async IAsyncEnumerable<List<UserModel>> LoadSourceModelBatchesAsync(int batchSize)
    {
        var modelEnumerator = this.LoadSourceModelsAsync().GetAsyncEnumerator();
        var batches = this.LoadSourceModelBatchesAsync(batchSize, modelEnumerator);
        this._logger.LogInformation("Loading the first batch...");
        await foreach (var batch in batches)
        {
            this._logger.LogInformation("Loaded batch successfully.");
            yield return batch;
            this._logger.LogInformation("Loading a new batch...");
        }
        this._logger.LogInformation("Batch load interrupted: no additional user models.");
    }
    protected async IAsyncEnumerable<List<UserModel>> LoadSourceModelBatchesAsync(int batchSize, IAsyncEnumerator<UserModel> enumerator)
    {
        if (enumerator == null)
            throw new ArgumentNullException(nameof(enumerator));
        List<UserModel> batch = null;
        while (await enumerator.MoveNextAsync())
        {
            if (batch == null)
                batch = new List<UserModel>();
            var model = enumerator.Current;
            batch.Add(model);
            if (batch.Count < batchSize)
                continue;
            yield return batch;
            batch = null;
        }
        if (batch != null)
            yield return batch;
    }
    protected async IAsyncEnumerable<UserModel> LoadModelsFromImageSourcesAsync()
    {
        if (this._imageset == null)
            throw new NullReferenceException
            (
                $"The {nameof(_imageset)} member reference has not been instantiated; "
                + "Verify that the active configuration instance specifies a image source directory "
                + "path as well as a ProfileImages import source."
            );
        await foreach (var model in this._imageset.GetModelsFromSourcesAsync())
            yield return model;
    }

    protected async IAsyncEnumerable<UserModel> LoadModelsFromSpreadsheetAsync()
    {
        if (this._spreadsheet == null)
            throw new NullReferenceException
            (
                $"The {nameof(_spreadsheet)} member reference has not been instantiated; "
                + "Verify that the active configuration instance specifies a valid CSV "
                + "path as well as a Spreadsheet import source."
            );
        await foreach (var model in this._spreadsheet.GetModelsFromSourcesAsync())
            yield return model;
    }

    protected async IAsyncEnumerable<UserModel> LoadSourceModelsAsync()
    {
        var enabledSources = this.Settings.ImportSources.SplitFlags();
        foreach (var source in enabledSources)
        {
            var models = source switch
            {
                ImportSources.Imagesets => this.LoadModelsFromImageSourcesAsync(),
                ImportSources.Spreadsheets => this.LoadModelsFromSpreadsheetAsync(),
                _ => throw new NotImplementedException()
            };
            await foreach (var model in models)
            {
                if (this.Settings.GenerateMissingData)
                {
                    model.Email = $"{model.LookupID.ToString("N")}@{this.Settings.DefaultClientEmailDomain}";
                }
                yield return model;
            }
        }
    }
}
