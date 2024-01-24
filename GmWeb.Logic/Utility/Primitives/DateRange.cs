using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions.Chronometry;
using System.Globalization;

namespace GmWeb.Logic.Utility.Primitives;

public record struct DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateRange(int startYear, int startMonth, int startDay)
        : this(new DateTime(startYear, startMonth, startDay))
    { }
    public DateRange(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        : this(new DateTime(startYear, startMonth, startDay), new DateTime(endYear, endMonth, endDay)) 
    { }
    public DateRange(DateTime start, DateTime end)
    {
        this.Start = start;
        this.End = end;
    }
    public DateRange(DateTime referenceDate) : this(TimePeriod.Daily, referenceDate) { }
    /// <summary>
    /// Creates a DateRange instance with parameterized time length and time offset.
    /// </summary>
    /// <param name="referenceDate">The reference date.</param>
    /// <param name="period">The time period used for <paramref name="periodShift"/> and <paramref name="periodScale"/>.</param>
    /// <param name="periodScale">The length of time encompassed by the date range in <paramref name="period"/> time units.</param>
    /// <param name="periodShift">The date range offset, in <paramref name="period"/> time units, relative to <paramref name="referenceDate"/>.</param>
    public DateRange(DateTime referenceDate, TimePeriod period, int periodScale = 1, int periodShift = 0)
    {
        this.Start = referenceDate
            .RecentPeriodStart(period)
            .NextPeriodStart(period, count: periodShift)
        ;
        this.End = referenceDate
            .NextPeriodStart(period, count: periodScale)
            .NextPeriodStart(period, count: periodShift)
        ;
        if (this.Start > this.End)
            (this.Start, this.End) = (this.End, this.Start);
    }

    /// <summary>
    /// Creates a DateRange instance with parameterized time length and time offset.
    /// </summary>
    /// /// <param name="period">The time period used for <paramref name="periodShift"/> and <paramref name="periodScale"/>.</param>
    /// <param name="referenceDate">The reference date.</param>
    /// <param name="periodScale">The length of time encompassed by the date range in <paramref name="period"/> time units.</param>
    /// <param name="periodShift">The date range offset, in <paramref name="period"/> time units, relative to <paramref name="referenceDate"/>.</param>
    public DateRange(TimePeriod period, DateTime? referenceDate = null, int periodScale = 1, int periodShift = 0)
        : this(referenceDate?.Date ?? DateTime.Today, period, periodScale, periodShift) { }

    /// <summary>
    /// Decrement the current date range's Start and End dates by a specified number of time periods.
    /// </summary>
    /// <param name="period">The time period used to determine the decrement offset.</param>
    /// <param name="count">The number of time periods by which the current range will be decremented.</param>
    /// <returns>A new <see cref="DateRange"/> instance offset by a time period of length <b>-1 × {<paramref name="count"/>} × {<paramref name="period"/>}</b></returns>
    public DateRange ShiftBackward(TimePeriod period, int count = 1)
        => this.ShiftForward(period, -count);

    /// <summary>
    /// Increment the current date range's Start and End dates by a specified number of time periods.
    /// </summary>
    /// <param name="period">The time period used to determine the increment offset.</param>
    /// <param name="count">The number of time periods by which the current range will be incremented.</param>
    /// <returns>A new <see cref="DateRange"/> instance offset by a time period of length <b>{<paramref name="count"/>} × {<paramref name="period"/>}</b></returns>
    public DateRange ShiftForward(TimePeriod period, int count=1)
        => new DateRange(
            start: this.Start.NextPeriodStart(period, count), 
            end: this.End.NextPeriodStart(period, count)
        );

    #region Comparison
    public bool Contains(DateTime dt)
    {
        return this.Start <= dt && dt < this.End;
    }
    public bool Contains(DateRange dr)
    {
        return this.Contains(dr.Start) && this.Contains(dr.End);
    }
    public static bool operator <=(DateRange left, DateRange right)
    {
        return left.Start <= right.Start;
    }
    public static bool operator >=(DateRange left, DateRange right)
    {
        return left.Start >= right.Start;
    }
    #endregion

    /// <summary>
    /// Get the sequence of <see cref="DateRange"/> instances, each having the time period <b><paramref name="period"/></b>,
    /// which comprise the same length of time as the current <see cref="DateRange"/> instance. 
    /// </summary>
    /// <example>
    /// For instance, calling
    /// <see cref="DateRange.SubRanges"/>(<see cref="TimePeriod.Monthly"/>) on a <see cref="DateRange"/> instance with 
    /// <see cref="DateRange.Start"/> = "2021-01-01" and <see cref="DateRange.End"/> = "2022-01-01"
    /// will produce the sequence of <see cref="DateRange"/> instances representing each month of 2021.
    /// </example>
    /// <param name="period">The time period of each <see cref="DateRange"/> instance in the sub-range.</param>
    /// <returns>The sequence of <see cref="DateRange"/> instances which comprise the sub-range.</returns>
    public List<DateRange> SubRanges(TimePeriod period)
    {
        DateTime subRangeStart = this.Start;
        DateTime subRangeEnd = this.End;
        if (period == TimePeriod.Weekly)
        {
            int isoStartWeek = ISOWeek.GetWeekOfYear(this.Start);
            int isoStartYear = ISOWeek.GetYear(this.Start);
            subRangeStart = ISOWeek.ToDateTime(isoStartYear, isoStartWeek, DayOfWeek.Monday);

            int isoEndWeek = ISOWeek.GetWeekOfYear(this.End);
            int isoEndYear = ISOWeek.GetYear(this.End);
            subRangeEnd = ISOWeek.ToDateTime(isoEndYear, isoEndWeek, DayOfWeek.Monday);
        }
        var ranges = new List<DateRange>
        {
            new DateRange(period, subRangeStart)
        };
        while(ranges[^1].End < subRangeEnd)
        {
            ranges.Add(new DateRange(period, ranges[^1].End));
        }
        return ranges;
    }
}
