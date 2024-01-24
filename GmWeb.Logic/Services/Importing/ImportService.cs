using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsyncContext = Nito.AsyncEx.AsyncContext;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Importing;
public abstract class ImportService
{
    public void Run() => AsyncContext.Run(this.RunAsync);
    public abstract Task RunAsync();
}
