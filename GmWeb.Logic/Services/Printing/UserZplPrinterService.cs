using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BinaryKits.Zpl.Viewer;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Common.Crypto;

namespace GmWeb.Logic.Services.Printing;

public class UserZplPrinterService : PrinterService<User>
{
    protected CarmaCache Cache { get; }

    public override PrinterCommandGenerator<User> Generator { get; protected set; }

    public UserZplPrinterService(CarmaContext context, IHostEnvironment env, IOptions<PrinterOptions> options, ILoggerFactory loggerFactory)
        : base(env, options, loggerFactory)
    {
        this.Cache = new CarmaCache(context);
        this.Generator = new UserZplGenerator(options);
    }

    public override async Task<IEnumerable<User>> GetPrintData()
    {
        var models = this.Cache.Users.AsQueryable();
        if (this.Options.ClientsOnly)
            models = models.Where(x => x.UserRole == Enums.UserRole.Client);
        if (this.Options.UseRecordStartIndex.HasValue)
            models = models.Skip(this.Options.UseRecordStartIndex.Value);
        if (this.Options.UseRecordCount.HasValue)
            models = models.Take(this.Options.UseRecordCount.Value);
        var processed = await models.ToListAsync();
        processed = processed
            .OrderBy(x => x.Profile.Residence.BuildingCode)
            .ThenBy(x => x.FullName)
            .ToList()
        ;
        return processed;
    }

    public override async Task<string> GetPrintPreviewAsync(User model)
    {
        IPrinterStorage printerStorage = new PrinterStorage();
        var drawer = new ZplElementDrawer(printerStorage);
        var command = this.Generator.CreateCommand(model);

        var analyzer = new ZplAnalyzer(printerStorage);
        var analyzeInfo = analyzer.Analyze(command);
        using var stream = new MemoryStream();
        foreach (var labelInfo in analyzeInfo.LabelInfos)
        {
            var imageData = drawer.Draw(labelInfo.ZplElements);
            await stream.WriteAsync(imageData, 0, imageData.Length);
        }

        return Base64.FromBytes(stream.ToArray());
    }

    public override string GetExportFilename(User model) => $"UID.{model.UserID:D4}.zpl";
}
