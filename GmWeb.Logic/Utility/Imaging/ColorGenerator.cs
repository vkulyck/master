using GmWeb.Logic.Utility.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vec = MathNet.Numerics.LinearAlgebra.Vector<int>;

namespace GmWeb.Logic.Utility.Imaging;
public class ColorGenerator
{
    public MetricDescriptor Descriptor { get; private set; }
    public Color LastColor { get; private set; }

    protected List<KnownColor> KnownColors = EnumExtensions.GetEnumValues<KnownColor>().ToList();
    private Random Random { get; set; } = new Random();

    private const int MaxColor = 0xFF;
    public ColorGenerator(MetricDescriptor descriptor)
    {
        this.Descriptor = descriptor;
        this.Next();
    }

    public Color Next()
    {
        int
            r = this.Random.Next(MaxColor),
            g = this.Random.Next(MaxColor),
            b = this.Random.Next(MaxColor)
        ;
        this.LastColor = Color.FromArgb(r, g, b);
        return this.LastColor;
    }

    public IEnumerable<Color> Next(int count)
    {
        var colors = new List<Color>();
        if (count <= this.KnownColors.Count)
        {
            // known color suggestion from TAW
            // https://stackoverflow.com/questions/37234131/how-to-generate-randomly-colors-that-is-easily-recognizable-from-each-other#comment61999963_37234131
            int step = (this.KnownColors.Count - 1) / count;
            int colorIndex = step;
            step = step == 0 ? 1 : step; // hacky
            for (int i = 0; i < count; i++)
            {
                var color = System.Drawing.KnownColors.FromKnownColor(this.KnownColors[colorIndex]);
                colors.Add(color);
                colorIndex += step;
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var color = this.GetNext();
                colors.Add(color);
            }
        }

        return colors;
    }

    public Color GetNext()
    {
        // use the previous value as a mix color as demonstrated by David Crow
        // https://stackoverflow.com/a/43235/578411
        var c = this.LastColor;
        var prev = Vec.Build.Dense(new int[] { c.R, c.G, c.B });
        int
            r = this.Random.Next(MaxColor),
            g = this.Random.Next(MaxColor),
            b = this.Random.Next(MaxColor)
        ;
        var delta = Vec.Build.Dense(new int[] { r, g, b });
        var next = (delta + prev) / 2;
        var color = Color.FromArgb(next[0], next[1], next[2]);
        this.LastColor = color;
        return color;
    }

}