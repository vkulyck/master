using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GmWeb.Logic.Utility.Extensions.Imaging;

public static class ImagingExtensions
{
    public static byte[] ToBytes(this Color color) => new byte[] { color.R, color.G, color.B, color.A };
}
