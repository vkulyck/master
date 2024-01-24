using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Extensions.Collections;

namespace GmWeb.Logic.Services.Printing;

public abstract class PrinterService<TModel>
{
    protected IHostEnvironment Env { get; }
    public PrinterOptions Options { get; }
    protected ILogger<PrinterService<TModel>> Logger { get; }
    public abstract PrinterCommandGenerator<TModel> Generator { get; protected set; }
    public PrinterService(IHostEnvironment env, IOptions<PrinterOptions> options, ILoggerFactory loggerFactory)
    {
        this.Env = env;
        this.Options = options.Value;
        this.Logger = loggerFactory.CreateLogger<PrinterService<TModel>>();
    }

    public abstract Task<IEnumerable<TModel>> GetPrintData();
    public abstract string GetExportFilename(TModel model);

    public virtual async Task RunAsync()
    {
        if (!this.Options.Enabled)
            return;
        if (this.Options.EnableExport)
            await this.ExportPrintCommandsAsync();
        if (this.Options.EnablePrinting)
            await this.QueuePrintCommandsAsync();
    }

    public virtual async Task ExportPrintCommandsAsync()
        => await this.ExportPrintCommandsAsync(await this.GetPrintData());
    public virtual async Task ExportPrintCommandsAsync<TCollection>(TCollection models)
        where TCollection : IEnumerable<TModel>
        => await models.ForEachAsync(this.ExportPrintCommandAsync);
    public async Task ExportPrintCommandAsync(TModel model)
    {
        if (!this.Options.EnableExport)
            return;
        var filename = this.GetExportFilename(model);
        var path = Path.Combine(this.Options.CommandExportDirectory, filename);
        var command = this.Generator.CreateCommand(model);
        using var writer = new StreamWriter(path);
        await writer.WriteAsync(command);
    }

    public virtual Task<string> GetPrintPreviewAsync(TModel model) => throw new NotImplementedException();

    public virtual async Task QueuePrintCommandsAsync()
        => await this.QueuePrintCommandsAsync(await this.GetPrintData());
    public virtual async Task QueuePrintCommandsAsync<TCollection>(TCollection models)
        where TCollection : IEnumerable<TModel>
        => await models.ForEachAsync(this.QueuePrintCommandAsync);

    public async Task QueuePrintCommandAsync(TModel model)
    {
        if (!this.Options.EnablePrinting)
            return;
        // Open connection
        using var tcpClient = new TcpClient();
        tcpClient.Connect(this.Options.PrinterHost, this.Options.PrinterPort);

        // Send Zpl data to printer
        using var writer = new StreamWriter(tcpClient.GetStream());
        using var reader = new StreamReader(tcpClient.GetStream());

        writer.AutoFlush = true;

        var command = this.Generator.CreateCommand(model);
        this.Logger.LogInformation($"Sending printer command: {command}");
        await writer.WriteLineAsync(command);

        var responseTask = reader.ReadToEndAsync();
        var responseTimeout = Task.Delay(this.Options.ResponseTimeout);
        var responseListener = Task.WhenAny(responseTask, responseTimeout);
        var listenResult = await responseListener;
        if (listenResult == responseTask)
        {
            var response = await responseTask;
            this.Logger.LogInformation($"Got printer response: {response}");
        }
    }
}
