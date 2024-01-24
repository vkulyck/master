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

namespace GmWeb.Logic.Services.QRCode;
public interface IQRCodeGeneratorService
{
    bool Enabled { get; }
    byte[] GenerateImage(TotpConfig config);
    string GenerateHtmlImageNode(TotpConfig config);
    void WriteQrCode(User user, string Filename = null, string OutputDirectory = null);
}