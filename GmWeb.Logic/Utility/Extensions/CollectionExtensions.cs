using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Utility.Extensions.Collections;
public static class CollectionExtensions
{
    public static async Task<HashSet<T>> ToHashSetAsync<T>(this IAsyncEnumerable<T> collection)
    {
        var hs = new HashSet<T>();
        await foreach (var item in collection)
            hs.Add(item);
        return hs;
    }
    public static async Task<HashSet<T>> ToHashSetAsync<T>(this IQueryable<T> collection)
        => (await collection.ToListAsync()).ToHashSet();

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
            action(item);
    }
    public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> action)
    {
        foreach (var item in collection)
            await action(item);
    }
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> collection, Func<T, Task> action)
    {
        await foreach (var item in collection)
            await action(item);
    }
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params T[] items) => Enumerable.Concat(collection, items);
    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> collection)
    {
        foreach (var item in collection)
            set.Add(item);
    }
    public static bool Matches<T>(this T item, params T[] others)
    {
        foreach (var other in others)
            if (item.Equals(other))
                return true;
        return false;
    }

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var p in pairs)
            dict[p.Key] = p.Value;
        return dict;
    }
    public static IDictionary<string, object> ToDictionary(this object data)
    {
        var publicAttributes = BindingFlags.Public | BindingFlags.Instance;
        var dictionary = new Dictionary<string, object>();

        foreach (var property in data.GetType().GetProperties(publicAttributes))
        {
            if (property.CanRead)
                dictionary.Add(property.Name, property.GetValue(data, null));
        }

        return dictionary;
    }

    public static Dictionary<T, T> Invert<T>(this Dictionary<T, T> dict)
    {
        var duplicates = dict.Values
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .ToList()
        ;
        if (duplicates.Count > 0)
        {
            string list = string.Join(", ", duplicates);
            throw new Exception($"Unable to invert dictionary due to {duplicates.Count} duplicate values: {list}");
        }
        return dict.ToDictionary(x => x.Value, x => x.Key, dict.Comparer);
    }

    // Works in C#3/VS2008:
    // Returns a new dictionary of this ... others merged leftward.
    // Keeps the type of 'this', which must be default-instantiable.
    // Example: 
    //   result = map.MergeLeft(other1, other2, ...)
    public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
        where T : IDictionary<K, V>, new()
    {
        var newMap = new T();
        var dictList = new List<IDictionary<K, V>> { me }.Concat(others);
        foreach (var src in dictList)
        {
            foreach (var p in src)
            {
                newMap[p.Key] = p.Value;
            }
        }
        return newMap;
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> items, params T[] others) => items.Except(others.ToList());

    public static byte[] XorSelf(this byte[] data, int ratio) => data.XorSelf(ratio, data.Length / ratio);
    public static byte[] XorSelf(this byte[] data, int ratio, int offset)
    {
        if (ratio <= 0)
            throw new ArgumentException($"Provided ratio must be positive. ({ratio} <= 0)");
        if (data.Length % ratio != 0)
            throw new ArgumentException($"Data length ({data.Length}) must be divisible by the provided ratio ({ratio})");
        byte[] result = new byte[data.Length / ratio];
        for (int i = 0; i < data.Length / ratio; i++)
        {
            result[i] = 0;
            for (int j = 0; j < ratio; j++)
            {
                result[i] ^= data[(i + j * offset) % data.Length];
            }
        }
        return result;
    }

    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static List<List<T>> ToTuples<T>(this IList<T> list, int tupleSize)
    {
        var tuples = new List<List<T>>();
        if (list.Count % tupleSize != 0)
            throw new Exception($"The input list cannot be converted to tuples of length {tupleSize}.");
        for (int i = 0; i < list.Count / tupleSize; i++)
        {
            var tuple = new List<T>();
            for (int j = 0; j < tupleSize; j++)
                tuple.Add(list[i * tupleSize + j]);
            tuples.Add(tuple);
        }
        return tuples;
    }

    public static List<List<T>> Zip<T>(this List<List<T>> lists)
    {
        var zipped = new List<List<T>>();
        for (int i = 0; i < lists[0].Count; i++)
        {
            zipped.Add(new List<T>());
            for (int j = 0; j < lists.Count; j++)
            {
                zipped.Last().Add(lists[j][i]);
            }
        }
        return zipped;
    }

    public static IEnumerable<TItem> Flatten<TItem>(this IEnumerable<IEnumerable<TItem>> subsets)
        => subsets.SelectMany(x => x);

    public static string JoinNonNull(this string separator, params string[] values)
        => JoinNonNull(separator, values.AsEnumerable());
    public static string JoinNonNull(this string separator, IEnumerable<string> values)
    {
        values = values.Where(x => !string.IsNullOrWhiteSpace(x));
        return string.Join(separator, values);
    }

    public static void AddRange<T>(this IList<T> list, params T[] items)
        => list.AddRange(items.AsEnumerable());
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
            list.Add(item);
    }

    /// <summary>
    /// Filters out items from the provided ccollection according to the filter selection predicate.
    /// </summary>
    /// <param name="items">The collection which will be filtered.</param>
    /// <param name="predicate">A filter selection function which returns true when it is invoked on an item that should be removed from the result.</param>
    /// <returns>The subset of <paramref name="items"/> for which each element <paramref name="item"/> satisfies the equation "predicate(item) == false".</returns>
    public static IEnumerable<T> ExceptWhere<T>(this IEnumerable<T> items, Func<T,bool> predicate)
    {
        return items.Where(item => !predicate(item));
    }

    public static IOrderedQueryable<T> Shuffle<T>(this IQueryable<T> items)
        => items.OrderBy(x => Guid.NewGuid());
}