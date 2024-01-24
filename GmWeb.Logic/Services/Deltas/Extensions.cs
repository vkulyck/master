using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Deltas;

internal static class Extensions
{
    /// <summary>
    /// JavaScript Splice function.
    /// </summary>
    public static List<T> Splice<T>(this List<T> input, int start, int count,
        params T[] objects)
    {
        List<T> deletedRange = input.GetRange(start, count);
        input.RemoveRange(start, count);
        input.InsertRange(start, objects);

        return deletedRange;
    }

    /// <summary>
    /// A variation on the Substring function with typical Java-style parameterization.
    /// </summary>
    public static string JavaSubstring(this string s, int begin, int end)
    {
        return s.Substring(begin, end - begin);
    }
}
