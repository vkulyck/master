using GmWeb.Logic.Utility.Extensions.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GmWeb.Logic.Utility.Imaging;
public abstract class MultiGradientBrush<TMetric> where TMetric : MetricDescriptor, new()
{
    public List<Color> ModeColors { get; set; }
    public List<double> Points { get; set; }
    public TMetric MetricDescriptor { get; private set; }
    public virtual int Modes => this.MetricDescriptor.Modes;
    public MultiGradientBrush(TMetric descriptor)
    {
        if (descriptor.Modes < 2)
            throw new ArgumentException($"A heatmap must have at least two modes.");
        this.MetricDescriptor = descriptor;
        this.Points = Enumerable.Range(0, descriptor.Modes)
            .Select(x => (double)x)
            .Select(x => x / (descriptor.Modes - 1))
            .ToList()
        ;
    }

    public static string GetNearestKnownColorName(Color color)
    {
        var knowns = EnumExtensions.GetEnumValues<KnownColor>();
        Func<KnownColor, double> ColorDistance = kc => (System.Drawing.KnownColors.FromKnownColor(kc).c2v() - color.c2v()).L2Norm();
        double minKC = knowns.Min(x => ColorDistance(x));
        var argmin = knowns.Where(x => ColorDistance(x) == minKC).FirstOrDefault();
        return argmin.ToString();
    }

    public (Color Color, int ModeIndex, double ProjectionScale, string StartColorName, string EndColorName)
        //public Color
        Interpolate(double metricValue)
    {
        double? proj = this.MetricDescriptor.Project(metricValue);
        if (proj == null)
            throw new Exception($"Cannot interpolate from a null projection.");
        for (int i = 0; i < this.Modes - 1; i++)
        {
            double pPoint = this.Points[i];
            double nPoint = this.Points[i + 1];
            if (pPoint >= nPoint)
                throw new Exception($"Gradient factors are not configured correctly; factors must be placed in strictly increasing order.");
            if (proj > nPoint)
                continue;
            if (proj < pPoint)
                throw new Exception($"Error processing heatmap gradient interpolation.");
            // Once we get here, pPoint < proj < nPoint
            double pointRange = nPoint - pPoint;
            double? pScale = (proj - pPoint) / pointRange;
            var pVec = this.ModeColors[i].c2v();
            var nVec = this.ModeColors[i + 1].c2v();
            var projVec = pVec + (nVec - pVec) * pScale.Value;
            var projColor = projVec.v2c();
            return (
                Color: projColor,
                ModeIndex: i,
                ProjectionScale: pScale.Value,
                StartColorName: GetNearestKnownColorName(this.ModeColors[i]),
                EndColorName: GetNearestKnownColorName(this.ModeColors[i + 1])
            );
            //return projColor;
        }
        throw new Exception($"Interpolation failed to find a midpoint.");
    }
}