using System;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.Options;
using QRCoder;
using QrDataGenerator = QRCoder.QRCodeGenerator;
using Formats = SixLabors.ImageSharp.Formats;
using DataCache = GmWeb.Logic.Data.Context.Carma.CarmaCache;
using DataContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using User = GmWeb.Logic.Data.Models.Carma.User;
using Base64 = GmWeb.Common.Crypto.Base64;
using ImageFormat = GmWeb.Logic.Enums.ImageFormat;
using GmWeb.Logic.Utility.Extensions.Imaging;

namespace GmWeb.Logic.Services.QRCode;
public class QRCodeGeneratorService : IQRCodeGeneratorService
{
    #region Properties
    protected DataCache Cache { get; private set; }
    protected QRCodeOptions Settings { get; set; }
    protected QrDataGenerator Generator { get; } = new QrDataGenerator();
    #endregion

    #region Constructors
    public QRCodeGeneratorService(DataContext context, IOptionsMonitor<QRCodeOptions> settings)
    {
        this.Cache = new DataCache(context);
        this.Settings = settings.CurrentValue;
        settings.OnChange((opts, name) =>
        {
            this.Settings = opts;
        });
    }
    #endregion

    #region Generators

    protected QRCodeData GeneratePayloadQRData(PayloadGenerator.Payload payload)
        => this.Generator.CreateQrCode(payload, this.Settings.ECCLevel);
    protected QRCodeData GenerateModelQRData(User user)
        => this.GeneratePayloadQRData(new UserPayload(user));
    protected QRCodeData GenerateModelQRData(TotpConfig config)
        => this.GeneratePayloadQRData(new TotpPayload(config));
    protected QRCodeData GenerateModelQRData<T>(T model)
    {
        switch(model)
        {
            case User user:
                return this.GenerateModelQRData(user);
            case TotpConfig config:
                return this.GenerateModelQRData(config);
            default:
                throw new NotImplementedException();
        }
    }
    protected byte[] GenerateImage<T>(T model)
    {
        switch(this.Settings.ImageFormat)
        {
            case ImageFormat.Png:
                return new PngByteQRCode(this.GenerateModelQRData(model)).GetGraphic(
                    this.Settings.PixelsPerModule,
                    this.Settings.DarkColor.ToBytes(),
                    this.Settings.LightColor.ToBytes(),
                    this.Settings.DrawQuietZones
                );
            case ImageFormat.Bmp:
                return new BitmapByteQRCode(this.GenerateModelQRData(model)).GetGraphic(
                    this.Settings.PixelsPerModule,
                    this.Settings.DarkColor.ToBytes(),
                    this.Settings.LightColor.ToBytes()
                );
            default:
                throw new NotImplementedException();
        }
    }
    #endregion

    #region IQRCodeGeneratorService
    public bool Enabled => this.Settings.Enabled;
    public byte[] GenerateImage(TotpConfig config)
        => this.GenerateImage<TotpConfig>(config);
    public byte[] GenerateImage(User user)
        => this.GenerateImage<User>(user);
    public string GenerateHtmlImageNode(TotpConfig config)
    {
        var imageData = this.GenerateImage<TotpConfig>(config);
        var qrEncodedImage = Base64.FromBytes(imageData);
        var htmlNode = $@"<img alt='Embedded QR Code' src='data:{this.Settings.ImageType.DefaultMimeType};base64,{qrEncodedImage}' />";
        return htmlNode;
    }
    public void WriteQrCode(User User, string Filename = null, string OutputDirectory = null)
    {
        Filename ??= $"{User.UserID:0000}_{User.LastName}_{User.FirstName}";
        Filename = Path.GetFileNameWithoutExtension(Filename);
        OutputDirectory ??= this.Settings.OutputDirectory;
        var image = this.GenerateImage(User);
        string path = Path.Combine(OutputDirectory, $"{Filename}.{this.Settings.FileExtension}");
        using (var stream = File.OpenWrite(path))
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(image);
            }
        }
    }
    #endregion
}