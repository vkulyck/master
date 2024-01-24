using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions.Enums;
using GmWeb.Logic.Utility.Extensions.Chronometry;
using GmWeb.Logic.Utility.Extensions.Collections;
using HalfGuid = GmWeb.Logic.Utility.Primitives.HalfGuid;
using DateGuid = GmWeb.Logic.Utility.Primitives.DateGuid;
using ActivityGuid = GmWeb.Logic.Data.Models.Carma.ActivityGuid;
using System.Globalization;

namespace GmWeb.Tests.Logic.Tests;

[Collection(nameof(LogicTestCollection))]
public class PrimitiveTests : LogicTestBase<PrimitiveTests>
{
    [Fact]
    public void TestHalfGuids()
    {
        var guid = Guid.NewGuid();
        var halfGuid = new HalfGuid(guid);

        IEnumerable<byte> tenBytes = new List<byte>();
        for (int i = 0; i < 10; i++)
        {
            var partBytes = Guid.NewGuid().ToByteArray();
            tenBytes = tenBytes.Concat(partBytes);
        }
        var tenArray = tenBytes.ToArray();
        byte[] xord = tenArray.XorSelf(10);
        var tenHG = new HalfGuid(tenArray);
        var xorHG = new HalfGuid(xord);
        Assert.Equal(tenHG, xorHG);
        Assert.Equal(HalfGuid.DataLength, tenHG.ToByteArray().Length);

        var guid_B = guid.ToString("B");
        var str_hg_n = new HalfGuid(guid_B).ToString("N");
        var str_g_n = guid.ToString("N").Substring(16);
        Assert.NotEqual(str_hg_n, str_g_n);
    }

    [Fact]
    public void TestDateGuids()
    {
        var g = Guid.NewGuid();
        var hg = new HalfGuid(g);
        var dt = DateTime.Now;
        var dg1 = new DateGuid(hg, dt);
        var bytes = dg1.ToByteArray();
        var guid = new Guid(bytes);
        var dg2 = new DateGuid(guid);
        Assert.Equal(dt, dg2.DateTime);
        Assert.Equal(hg, dg2.HalfGuid);
    }

    [Fact]
    public void TestActivityDateGuids()
    {
        var uid = Guid.NewGuid().ToString("D");
        var date = DateTime.Now;
        var activity_id_creator = new ActivityGuid(uid, date);
        var activity_id_query = new ActivityGuid(activity_id_creator.ActivityID);
        var event_id_selector = new HalfGuid(uid);
        var event_id_query = activity_id_query.EventID;
        Assert.Equal(event_id_query, event_id_selector);
    }

    [Fact]
    public void TestSubRanges()
    {
        var range = new DateRange(2021, 1, 1, 2021, 12, 31);
        var periods = EnumExtensions.GetEnumValues<TimePeriod>();
        for (int i = 1; i < periods.Count; i++)
        {
            var period = periods[i];
            var subPeriod = period.SubPeriod();
            var subRanges = range.SubRanges(subPeriod);
            for (int j = 0; j < subRanges.Count; j++)
            {
                var expRange = new DateRange(range.Start.NextPeriodStart(subPeriod, j), subPeriod);
                var subRange = subRanges[j];
                Assert.Equal(expRange, subRange);
            }
            switch (subPeriod)
            {
                case TimePeriod.Weekly:
                    var expWeeks = ISOWeek.GetWeeksInYear(range.Start.Year);
                    Assert.Equal(expWeeks, subRanges.Count);
                    var firstWeekYear = ISOWeek.GetYear(range.Start);
                    var firstWeekNumber = ISOWeek.GetWeekOfYear(range.Start);
                    var subRangeStart = ISOWeek.ToDateTime(firstWeekYear, firstWeekNumber, DayOfWeek.Monday);
                    Assert.Equal(subRangeStart, subRanges[0].Start);
                    var lastWeekYear = ISOWeek.GetYear(range.End);
                    var lastWeekNumber = ISOWeek.GetWeekOfYear(range.End);
                    var subRangeEnd = ISOWeek.ToDateTime(lastWeekYear, lastWeekNumber, DayOfWeek.Monday);
                    Assert.Equal(subRangeEnd, subRanges[^1].End);
                    break;
                case TimePeriod.Monthly:
                    Assert.Equal(12, subRanges.Count);
                    for (int month = 1; month <= 12; month++)
                    {
                        Assert.Equal(new DateTime(2021, month, 01), subRanges[month - 1].Start);
                        Assert.Equal(new DateTime(2021, month, 01).AddMonths(1), subRanges[month - 1].End);
                    }
                    break;
                case TimePeriod.Quarterly:
                    Assert.Equal(4, subRanges.Count);
                    for (int qtr = 0; qtr < 4; qtr++)
                    {
                        Assert.Equal(new DateTime(2021, qtr * 3 + 1, 01), subRanges[qtr].Start);
                        Assert.Equal(new DateTime(2021, qtr * 3 + 1, 01).AddQuarters(1), subRanges[qtr].End);
                    }
                    break;
            }
        }
    }
}