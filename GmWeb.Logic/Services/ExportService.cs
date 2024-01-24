using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GmWeb.Logic.Services;

public interface IExportOptions
{
    string OutputDirectory { get; }
}
public interface IExportService<TOptions> where TOptions : IExportOptions
{
    IHostEnvironment Env { get; }
    TOptions Options { get; }
    
    Task Export(string OutputFilename = null, string OutputDirectory = null, string OutputPath = null);

    #region Path Resolution

    protected static readonly Func<string, string> ExtraArgErrorMessage = (string paramName) =>
     $"{paramName} and Path arguments provided simultaneously; either provide Path, Filename, or Directory and Filename arguments.";
    protected static readonly Func<string, string> MissingArgErrorMessage = (string paramName) =>
         $"Neither {paramName} nor Path arguments provided; either provide Path, Filename, or Directory and Filename arguments.";
    string GetExporOutputPath(string OutputFilename = null, string OutputDirectory = null, string OutputPath = null)
    {
        if (string.IsNullOrWhiteSpace(OutputPath))
        {
            if (string.IsNullOrWhiteSpace(OutputFilename))
                throw new ArgumentException(MissingArgErrorMessage(nameof(OutputFilename)), nameof(OutputFilename));
            if (string.IsNullOrWhiteSpace(OutputDirectory))
                OutputDirectory = this.Options.OutputDirectory ?? this.Env.ContentRootPath;
            OutputPath = Path.Combine(OutputDirectory, OutputFilename);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(OutputFilename))
                throw new ArgumentException(ExtraArgErrorMessage(nameof(OutputFilename)), nameof(OutputFilename));
            if (!string.IsNullOrWhiteSpace(OutputDirectory))
                throw new ArgumentException(ExtraArgErrorMessage(nameof(OutputDirectory)), nameof(OutputDirectory));
        }
        return OutputPath;
    }

    #endregion
}
