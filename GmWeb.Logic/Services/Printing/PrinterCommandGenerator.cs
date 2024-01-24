using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace GmWeb.Logic.Services.Printing;

public abstract class PrinterCommandGenerator<TModel>
{
    protected PrinterOptions Options { get; }
    public PrinterCommandGenerator(IOptions<PrinterOptions> options)
    {
        this.Options = options.Value;
    }
    public abstract string CreateCommand(TModel model);

    public virtual List<string> CreateCommands(IEnumerable<TModel> models)
    {
        return models.Select(model => this.CreateCommand(model)).ToList();
    }
}
