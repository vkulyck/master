using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GmWeb.Web.Demographics.ViewModels.Geo
{
    public class MedianIncomeDescriptor : GmWeb.Logic.Utility.Math.MetricDescriptor
    {
        public override double LowerBound => 0;
        public override double UpperBound => 250000;
        public override int Modes => 5;
    }
}