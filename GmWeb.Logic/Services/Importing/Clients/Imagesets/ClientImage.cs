using ImageMagick;
using System;
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
public class ClientImage : IClientSource
{
    #region Properties
    public string Filename => $"{this.Basename}{this.Extension}";
    public string Basename { get; protected set; }
    public string Extension { get; protected set; }
    public byte[] ImageData { get; protected set; }

    public int? Height { get; protected set; }
    public int? Width { get; protected set; }
    public int? PixelCount => this.Height * this.Width;
    public ClientImportOptions Settings { get; protected set; }
    public bool ImageValidated { get; protected set; }

    #endregion
    public ClientImage(string path, ClientImportOptions settings)
    {
        this.Basename = SysPath.GetFileNameWithoutExtension(path);
        this.Extension = SysPath.GetExtension(path);
        this.Settings = settings;
    }

    #region Client Model Assembly

    public async Task<ClientModel> GetModelFromSourceAsync(ClientImportOptions options, CarmaContext context)
    {
        var client = new ClientModel { AgencyID = options.AgencyID };
        if (!this.ExtractExplicitFilenameData(client))
            if (!this.ExtractImplicitFilenameData(client))
                return default;
        await context.Users.AddAsync(client);
        await context.SaveAsync();
        client.GenerateLookupID(client.UserID);
        return client;
    }

    protected bool ExtractExplicitFilenameData(ClientModel model)
    {
        var pattern = new Regex
        (
            @"^(?<BuildingCode>[A-Z]{2,4})[_ ]+(?<UnitNumber>\d+)(?:[ _]+)(?:F ?[-_] ?)?(?<FirstName>[\w -]+)(?:[ _,\.]+)(?:L[-_])?(?<LastName>[\w ]+)"
        );
        return this.ExtractFilenameData(model, pattern);
    }

    protected bool ExtractImplicitFilenameData(ClientModel model)
    {
        var pattern = new Regex
        (
            @"^(?<BuildingCode>[A-Z]{2,4})[_ ]+(?<UnitNumber>\d+)(?:[ _]+)(?<FirstName>[\w- ]+?)(?:[ _]+)(?<LastName>\w+)$"
        );
        return this.ExtractFilenameData(model, pattern);
    }

    protected bool ExtractFilenameData(ClientModel model, Regex pattern)
    {
        var match = pattern.Match(this.Basename);
        if (!match.Success)
            return false;
        model.FirstName = match.Groups["FirstName"].Value;
        model.LastName = match.Groups["LastName"].Value;
        model.Profile.Residence.BuildingCode = match.Groups["BuildingCode"].Value;
        model.Profile.Residence.UnitNumber = match.Groups["UnitNumber"].Value;

        // Many of the client files in the import dataset specify FirstName values with underscores.
        // Multiple names are allowed in the FirstName field, so we convert these underscores to spaces
        // until alternative specifications are provided.
        model.FirstName = model.FirstName.Replace("_", " ");
        return true;
    }
    #endregion

    #region Image Processing
    protected async Task<byte[]> ReadFileAsync(string filePath)
    {
        byte[] fileData;
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        {
            fileData = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileData, 0, (int)fileStream.Length);
        }
        return fileData;
    }
    public async Task NormalizeImageAsync(ClientModel model, string sourcePath, ProcessingFlags processing)
    {
        string targetPath = SysPath.Combine(this.Settings.ProcessedImageDirectory, $"{model.LookupID}.jpg");
        if (File.Exists(targetPath))
            return;
        SysDir.CreateDirectory(SysPath.GetDirectoryName(targetPath));
        File.Copy(sourcePath, targetPath, true);

        if (processing.HasFlag(ProcessingFlags.Compress))
        {
            var optimizer = new ImageOptimizer();
            optimizer.Compress(targetPath);
        }
        this.ImageData = await this.ReadFileAsync(targetPath);
        var image = new MagickImage(this.ImageData);
        if (processing.HasFlag(ProcessingFlags.Resize))
        {
            image.InterpolativeResize(this.Settings.MaximumWidth.Value, this.Settings.MaximumHeight.Value, PixelInterpolateMethod.Mesh);
            await image.WriteAsync(targetPath, MagickFormat.Jpeg);
        }
        this.Height = image.Height;
        this.Width = image.Width;
    }
    #endregion

    public override string ToString() => $"{this.Filename}: {this.Width}x{this.Height}";
}