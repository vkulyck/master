using System;
using System.Collections.Generic;
using System.Linq;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Api.Utility;

public class ActivitySettings
{
    public int GetMaximumPeriodCount(TimePeriod period)
    {
        if (this.MaximumListPeriodCount.TryGetValue(period, out int count))
            return count;
        return default;
    }
    public int GetMaximumPeriodIndex(TimePeriod period, int currentIndex)
    {
        return currentIndex - 1 + this.GetMaximumPeriodCount(period);
    }
    public Dictionary<TimePeriod, int> MaximumListPeriodCount { get; set; }
    public int? GetTargetActivityCount(TimePeriod period)
    {
        if (this.TargetActivityCount.TryGetValue(period, out int? count))
            return count;
        return default;
    }
    public Dictionary<TimePeriod, int?> TargetActivityCount { get; set; }
}
