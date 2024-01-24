using System;
using System.Collections.Generic;
using System.Text;

namespace NCalc
{
    static internal class Extensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection)
        {
            var hset = new HashSet<T>();
            foreach (var item in collection)
                hset.Add(item);
            return hset;
        }
    }
}
