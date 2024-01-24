using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Primitives;

namespace GmWeb.Logic.Data.Models.Carma;

public record class ActivityGuid : DateGuid
{
    public DateTime ActivityStartTime => base.DateTime;
    public HalfGuid EventID => base.HalfGuid;
    public Guid ActivityID => base.Guid;
    public DateRange FilterRange { get; }
    public ActivityGuid(Guid activityID) : base(activityID)
    {
        this.FilterRange = new DateRange(this.ActivityStartTime);
    }
    public ActivityGuid(DateGuid dg) : base(dg)
    {
        this.FilterRange = new DateRange(this.ActivityStartTime);
    }
    public ActivityGuid(string eventUid, DateTime startTime) : base(eventUid, startTime)
    {
        this.FilterRange = new DateRange(this.ActivityStartTime);
    }
}
