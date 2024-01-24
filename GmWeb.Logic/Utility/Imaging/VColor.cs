using System.Drawing;
using Vec = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GmWeb.Logic.Utility.Imaging;
public class VColor : Vec
{
    public Color Color { get; private set; }
    public VColor(int r, int g, int b) : base(new double[] { r, g, b })
    {
        this.Color = Color.FromArgb(r, g, b);
    }

    public VColor(string hex) : base(ColorTranslator.FromHtml(hex).c2a()) { }

    public static VColor FromHex(string hex) => new VColor(hex);
}