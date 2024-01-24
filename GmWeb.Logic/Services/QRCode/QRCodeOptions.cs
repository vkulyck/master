using System;
using System.Linq;
using QRCoder;
using SixLabors.ImageSharp.Formats;
using System.Drawing;
using Bmp = SixLabors.ImageSharp.Formats.Bmp.BmpFormat;
using Png = SixLabors.ImageSharp.Formats.Png.PngFormat;
using Jpeg = SixLabors.ImageSharp.Formats.Jpeg.JpegFormat;
using Gif = SixLabors.ImageSharp.Formats.Gif.GifFormat;
using GmWeb.Logic.Enums;
using Newtonsoft.Json;

namespace GmWeb.Logic.Services.QRCode;
public class QRCodeOptions : ServiceOptions
{
    /// <summary>
    /// The default directory to which QR code images will be saved during write operations.
    /// </summary>
    public string OutputDirectory { get; set; }
    /// <summary>
    /// The number of pixels used to represent each of the 21x21 modules in a QR code.
    /// </summary>
    public int PixelsPerModule { get; set; } = 10;
    /// <summary>
    /// The width and height of the generated square QR code image.
    /// </summary>
    public int? ImageSize { get; set; } = 150;
    /// <summary>
    /// Enable this property to include quiet zones. Quiet Zones are defined as, "the blank
    /// margin on the either side of a barcode that's used to tell the barcode scanner where a
    /// barcode's symbol starts and stops."
    /// </summary>
    public bool DrawQuietZones { get; set; } = true;
    /// <summary>
    /// The format of any generated QR code images. Currently only Png and Bmp 
    /// are supported.
    /// </summary>
    public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;
    /// <summary>
    /// The method used for compressing the QR code's payload data.
    /// </summary>
    public QRCodeData.Compression Compression { get; set; } = QRCodeData.Compression.GZip;
    /// <summary>
    /// The level of error correction supported by the QR code. From least to greatest error
    /// resilience, these are: L (7%), M (15%), Q (25%), and H (30%).
    /// </summary>
    public QRCodeGenerator.ECCLevel ECCLevel { get; set; } = QRCodeGenerator.ECCLevel.H;
    /// <summary>
    /// The color used for the dark portion of the QR code. By default, this color is typically 
    /// white.
    /// </summary>
    public Color DarkColor { get; set; } = Color.Black;
    /// <summary>
    /// The color used for the dark portion of the QR code. By default, this color is typically 
    /// white.
    /// </summary>
    public Color LightColor { get; set; } = Color.White;

    [JsonIgnore]
    public IImageFormat ImageType
    {
        get
        {
            switch (this.ImageFormat)
            {
                case ImageFormat.Png:
                    return Png.Instance;
                case ImageFormat.Bmp:
                    return Bmp.Instance;
                case ImageFormat.Jpeg:
                    return Jpeg.Instance;
                case ImageFormat.Gif:
                    return Gif.Instance;
                default:
                    throw new NotImplementedException();
            }
        }
    }
    public string FileExtension => this.ImageType.FileExtensions.FirstOrDefault();
}