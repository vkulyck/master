using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GmWeb.Logic.Utility.Imaging;
public class PaletteBrush<TMetric> : MultiGradientBrush<TMetric> where TMetric : MetricDescriptor, new()
{
    public override int Modes => this.ModeColors.Count;
    public PaletteBrush(TMetric descriptor) : base(descriptor)
    {
        var pairs = new List<(Color, double)>
            {
                (Color.Indigo, 0D),
                (Color.DarkBlue, 0.2D),
                (Color.Cyan, 0.4D),
                (Color.SpringGreen, 0.45),
                (Color.Gold, 0.55),
                (Color.Red, 0.6D),
                (Color.Maroon, 0.8D),
                (Color.Snow, 1D),
            };
        this.ModeColors = pairs.Select(x => x.Item1).ToList();
        this.Points = pairs.Select(x => x.Item2).ToList();
    }
}