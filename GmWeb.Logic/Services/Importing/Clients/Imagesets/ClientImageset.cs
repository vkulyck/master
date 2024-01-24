using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using SysDir = System.IO.Directory;
using SysPath = System.IO.Path;
using UserModel = GmWeb.Logic.Data.Models.Carma.User;
using ClientModel = GmWeb.Logic.Data.Models.Carma.User;

namespace GmWeb.Logic.Services.Importing.Clients.Imagesets;
public class ClientImageset : IClientReader
{
    #region Properties
    public ProcessingFlags Processing
    {
        get
        {
            var processing = ProcessingFlags.None;
            if (this.Settings.EnableCompression)
                processing = processing | ProcessingFlags.Compress;
            if (this.Settings.MaximumWidth.HasValue || this.Settings.MaximumHeight.HasValue)
                processing = processing | ProcessingFlags.Resize;
            return processing;
        }
    }
    public ClientImportOptions Settings { get; }
    protected CarmaContext Context { get; }

    #endregion
    public ClientImageset(ClientImportOptions settings, CarmaContext context)
    {
        this.Settings = settings;
        this.Context = context;
    }

    public async IAsyncEnumerable<ClientModel> GetModelsFromSourcesAsync()
    {
        string root = this.Settings.SourceImageDirectory;
        if (!Path.IsPathRooted(root))
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            root = Path.Combine(assemblyDirectory, root);
        }
        var enumerator = Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories);
        foreach (string path in enumerator)
        {
            var model = await this.ProcessSourceDataAsync(path);
            if (model == null)
                continue;
            yield return model;
        }
    }

    protected async Task<ClientModel> ProcessSourceDataAsync(string sourcePath)
    {
        if (!File.Exists(sourcePath))
            return null;
        var clientImage = new ClientImage(sourcePath, this.Settings);
        if (!Regex.IsMatch(clientImage.Extension, @"\.jpe?g", RegexOptions.IgnoreCase))
            return null;
        try
        {
            var clientModel = await clientImage.GetModelFromSourceAsync(this.Settings, this.Context);
            if (clientModel == null)
                return default;
            await clientImage.NormalizeImageAsync(clientModel, sourcePath, this.Processing);
            return clientModel;
        }
        catch (Exception ex)
        {
            if (ex.Message == "Not a JPEG file: starts with 0x00 0x00")
                return null;
            if (ex.Message == "Parameter is not valid.")
                return null;
            throw;
        }
    }
}