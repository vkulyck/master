using System;
using System.Globalization;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Primitives;

namespace GmWeb.Logic.Utility.Extensions.Chronometry;

public static class TimePeriodExtensions
{
    #region Special Period Navigators

    #region Special Period Predecessors
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the previous Sunday, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next Sunday.</param>
    /// <param name="count">The number of periods to count backward.</param>
    /// <returns>The start of the Sunday falling strictly after the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime PrevioustWeekStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentWeekStart().AddWeeks(-count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next month, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next month.</param>
    /// <param name="count">The number of periods to count backward.</param>
    /// <returns>The start of the month strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime PreviousMonthStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentMonthStart().AddMonths(-count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next quarter, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next quarter.</param>
    /// <param name="count">The number of periods to count backward.</param>
    /// <returns>The start of the quarter strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime PreviousQuarterStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentQuarterStart().AddMonths(-3 * count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next year, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next year.</param>
    /// <param name="count">The number of periods to backward.</param>
    /// <returns>The start of the year strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime PreviousYearStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentYearStart().AddYears(-count);
    #endregion
    
    #region Special Period Initiators
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the most recent Monday, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the most recent Monday.</param>
    /// <returns>The start of the Monday falling on or before the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime RecentWeekStart(this DateTime referenceDate)
    {
        int isoYear = ISOWeek.GetYear(referenceDate);
        int isoWeek = ISOWeek.GetWeekOfYear(referenceDate);
        return ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Monday);
    }
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the current month, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the current month.</param>
    /// <returns>The start of the current month, relative to the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime RecentMonthStart(this DateTime referenceDate)
        => referenceDate.AddDays(1 - referenceDate.Day).Date;
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the current quarter, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the current quarter.</param>
    /// <returns>The start of the current quarter, relative to the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime RecentQuarterStart(this DateTime referenceDate)
    {
        var monthStart = referenceDate.RecentMonthStart();
        var moq = referenceDate.MonthOfQuarter();
        var monthOffset = 1 - moq;
        var result = monthStart.AddMonths(monthOffset);
        return result;
    }
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the current year, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the current year.</param>
    /// <returns>The start of the current year, relative to the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime RecentYearStart(this DateTime referenceDate)
        => new DateTime(referenceDate.Year, 1, 1);
    #endregion

    #region Special Period Successors
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next Sunday, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next Sunday.</param>
    /// <param name="count">The number of periods to count forward.</param>
    /// <returns>The start of the Sunday falling strictly after the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime NextWeekStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentWeekStart().AddWeeks(count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next month, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next month.</param>
    /// <param name="count">The number of periods to count forward.</param>
    /// <returns>The start of the month strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime NextMonthStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentMonthStart().AddMonths(count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next quarter, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next quarter.</param>
    /// <param name="count">The number of periods to count forward.</param>
    /// <returns>The start of the quarter strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime NextQuarterStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentQuarterStart().AddMonths(3 * count);
    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next year, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next year.</param>
    /// <param name="count">The number of periods to count forward.</param>
    /// <returns>The start of the year strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime NextYearStart(this DateTime referenceDate, int count = 1)
        => referenceDate.RecentYearStart().AddYears(count);

    #endregion

    #endregion

    #region General Period Navigators
    /// <summary>
    /// Calculates the DateTime value corresponding to the most recent time period start, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the most recent time period start.</param>
    /// <param name="count">The number of periods to count backward.</param>
    /// <returns>The start of the time period falling on or before the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime PreviousPeriodStart(this DateTime referenceDate, TimePeriod period, int count = 1)
    {
        switch (period)
        {
            case TimePeriod.Daily:
                return referenceDate.Date.AddDays(-count);
            case TimePeriod.Weekly:
                return referenceDate.PrevioustWeekStart(count);
            case TimePeriod.Monthly:
                return referenceDate.PreviousMonthStart(count);
            case TimePeriod.Quarterly:
                return referenceDate.PreviousQuarterStart(count);
            case TimePeriod.Yearly:
                return referenceDate.PreviousYearStart(count);
            default:
                throw new ArgumentOutOfRangeException(nameof(period), $"Invalid time period specified: {period}");
        }
    }

    /// <summary>
    /// Calculates the DateTime value corresponding to the most recent time period start, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the most recent time period start.</param>
    /// <returns>The start of the time period falling on or before the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime RecentPeriodStart(this DateTime referenceDate, TimePeriod period)
    {
        switch (period)
        {
            case TimePeriod.Daily:
                return referenceDate.Date;
            case TimePeriod.Weekly:
                return referenceDate.RecentWeekStart();
            case TimePeriod.Monthly:
                return referenceDate.RecentMonthStart();
            case TimePeriod.Quarterly:
                return referenceDate.RecentQuarterStart();
            case TimePeriod.Yearly:
                return referenceDate.RecentYearStart();
            default:
                throw new ArgumentOutOfRangeException(nameof(period), $"Invalid time period specified: {period}");
        }
    }


    /// <summary>
    /// Calculates the DateTime value corresponding to the start of the next time period, relative to the <paramref name="referenceDate"/> parameter.
    /// </summary>
    /// <param name="referenceDate">The point in time used to determine the next time period.</param>
    /// <param name="count">The number of periods to count forward.</param>
    /// <returns>The start of the time period strictly following the <paramref name="referenceDate"/> parameter.</returns>
    public static DateTime NextPeriodStart(this DateTime referenceDate, TimePeriod period, int count = 1)
    {
        switch (period)
        {
            case TimePeriod.Daily:
                return referenceDate.Date.AddDays(count);
            case TimePeriod.Weekly:
                return referenceDate.NextWeekStart(count);
            case TimePeriod.Monthly:
                return referenceDate.NextMonthStart(count);
            case TimePeriod.Quarterly:
                return referenceDate.NextQuarterStart(count);
            case TimePeriod.Yearly:
                return referenceDate.NextYearStart(count);
            default:
                throw new ArgumentOutOfRangeException(nameof(period), $"Invalid time period specified: {period}");
        }
    }

    public static Quarter Quarter(this DateTime dt) => (Quarter)(((dt.Month - 1) / 3) + 1);
    public static int MonthOfQuarter(this DateTime dt) => 1 + (dt.Month - 1) % 3;

    public static TimePeriod SubPeriod(this TimePeriod period, int levels = 1)
    {
        int iPeriod = (int)period;
        int iSubPeriod = iPeriod - levels;
        TimePeriod subPeriod = (TimePeriod)iSubPeriod;
        return subPeriod;
    }

    #endregion

    #region General Period Comparators

    public static int PeriodCount(this TimePeriod period, DateRange? range)
        => range.HasValue ? period.PeriodCount(range.Value) : 0;
    public static int PeriodCount(this TimePeriod period, DateRange range)
        => period.PeriodCount(range.Start, range.End);
    public static int PeriodCount(this TimePeriod period, DateTime? date)
        => date.HasValue ? period.PeriodCount(date.Value) : 0;
    public static int PeriodCount(this TimePeriod period, DateTime date)
        => period.PeriodCount(DateTime.Now, date);
    public static int PeriodCount(this TimePeriod period, DateTime start, DateTime end)
    {
        int offset = 0;
        if (start > end)
            (end, start) = (start, end);
        var current = start.RecentPeriodStart(period);
        var periodEnd = end.RecentPeriodStart(period);
        while(current < periodEnd)
        {
            offset++;
            current = current.NextPeriodStart(period);
        }
        return offset;
    }

    #endregion
}
