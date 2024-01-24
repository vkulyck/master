using System.Linq;

namespace GmWeb.Logic.Utility.Imaging;
public class RandomHeatmapBrush<TMetric> : MultiGradientBrush<TMetric> where TMetric : MetricDescriptor, new()
{
    protected ColorGenerator ColorGenerator { get; private set; }
    public RandomHeatmapBrush(TMetric descriptor) : base(descriptor)
    {
        this.ColorGenerator = new ColorGenerator(this.MetricDescriptor);
        this.ModeColors = this.ColorGenerator.Next(this.Modes + 1).ToList();
    }
}