using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;
using VecDouble = MathNet.Numerics.LinearAlgebra.Vector<double>;

namespace GmWeb.Logic.Utility.Imaging;
public static class ColorVectorizationExtensions
{
    public static Vector c2v(this Color c)
    {
        var v = Vector.Build.Dense(new double[] { c.R, c.G, c.B });
        return v as Vector;
    }

    public static Color v2c(this Vector v)
    {
        var c = Color.FromArgb((int)v[0], (int)v[1], (int)v[2]);
        return c;
    }
    public static Color v2c(this VecDouble v)
    {
        var c = Color.FromArgb((int)v[0], (int)v[1], (int)v[2]);
        return c;
    }

    public static double[] c2a(this Color c)
    {
        var v = c.c2v();
        return v.ToArray();
    }


    public static string ToHex(this Vector v)
        => "#" + ((int)v[0]).ToString("X2") + ((int)v[1]).ToString("X2") + ((int)v[2]).ToString("X2")
    ;
    public static Color v2c(this VColor v)
    {
        var c = Color.FromArgb((int)v[0], (int)v[1], (int)v[2]);
        return c;
    }
}