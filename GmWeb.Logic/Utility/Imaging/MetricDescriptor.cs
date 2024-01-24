using SysMath = System.Math;

namespace GmWeb.Logic.Utility.Imaging;
public abstract class MetricDescriptor
{
    // TODO: Consider multidimensional metrics
    public abstract double LowerBound { get; }
    public abstract double UpperBound { get; }
    public abstract int Modes { get; }
    public double Diameter => System.Math.Abs(this.UpperBound - this.LowerBound);

    public virtual double? Project(decimal? metricValue)
        => this.Project((double?)metricValue);
    public virtual double? Project(double? metricValue)
    {
        if (metricValue == null)
            return null;
        double p = (metricValue.Value - this.LowerBound) / this.Diameter;
        p = SysMath.Min(1.0D, SysMath.Max(0.0D, p));
        return p;
    }
}