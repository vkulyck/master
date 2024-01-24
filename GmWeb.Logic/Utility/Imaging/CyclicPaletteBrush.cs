using System.Collections.Generic;
using System.Drawing;


namespace GmWeb.Logic.Utility.Imaging;
public class CyclicPaletteBrush<TMetric> : MultiGradientBrush<TMetric> where TMetric : MetricDescriptor, new()
{
    public override int Modes => this.ModeColors.Count;
    public CyclicPaletteBrush(TMetric descriptor) : base(descriptor)
    {
        var pairs = new List<(string, double)>
            {
                    ("#fcfc81",0),
                    ("#ffd75f",.1),
                    ("#ffb14c",.2),
                    ("#ff8847",.3),
                    ("#f95e4d",.4),
                    ("#e82c59",.5),
                    ("#cc0068",.6),
                    ("#a60077",.7),
                    ("#720084",.8),
                    ("#0b008b",1.0)
            };
        this.ModeColors = new List<Color>();
        this.Points = new List<double>();
        foreach (var pair in pairs)
        {
            var color = ColorTranslator.FromHtml(pair.Item1);
            this.ModeColors.Add(color);

            double point = pair.Item2;
            this.Points.Add(point);
        }
    }
}