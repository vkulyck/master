using GmWeb.Logic.Utility.Extensions.Collections;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Xml;
using IAttrMap = System.Collections.Generic.IDictionary<string, object>;

namespace GmWeb.Logic.Utility.Extensions.Html
{
    //public interface IAttrMap : IDictionary<string, object> { }
    public class StylePair
    {
        public KeyValuePair<string, string> Pair { get; set; }
        public StylePair(string key, string value)
        {
            this.Pair = new KeyValuePair<string, string>(key, value);
        }
        public static implicit operator StylePair(KeyValuePair<string, string> pair)
            => new StylePair(pair.Key, pair.Value);
        public static implicit operator KeyValuePair<string, string>(StylePair stylePair)
            => stylePair.Pair;
    }
    public class StyleMap : Dictionary<string, string>
    {
        public StyleMap() { }
        public StyleMap(IEnumerable<KeyValuePair<string, string>> items)
            : base(items.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase))
        { }
        public static implicit operator StyleMap(string styleTag)
            => styleTag.ToStyleMap();
        public static implicit operator string(StyleMap styleMap)
            => styleMap.ToStyleTag();
    }
    public class StyleMapCollection : List<StyleMap>
    {
        public StyleMapCollection() { }
        public StyleMapCollection(IEnumerable<StyleMap> styleMaps)
        {
            this.AddRange(styleMaps);
        }
        public StyleMapCollection(IEnumerable<string> styleMaps)
        {
            this.AddRange(styleMaps.Cast<StyleMap>());
        }
        public static implicit operator StyleMapCollection(string[] styleMaps)
            => new StyleMapCollection(styleMaps);
        public static implicit operator StyleMapCollection(StyleMap[] styleMaps)
            => new StyleMapCollection(styleMaps);
    }
    public class ClassSet : HashSet<string>
    {
        public ClassSet() { }
        public ClassSet(IEnumerable<string> items) : base(items) { }
        public static implicit operator ClassSet(string classTag) => classTag.ToClassSet();
    }
    public enum ConflictBehavior
    {
        Overwrite,
        Skip,
        KeepLeft,
        KeepRight
    }
    public static class HtmlExtensions
    {
        public static ClassSet ToClassSet(this IEnumerable<string> collection)
            => new ClassSet(collection);
        public static object MergeAttributes(this object head, params object[] tail)
            => MergeAttributes(head, ConflictBehavior.KeepLeft, tail);
        public static object MergeAttributes(this object head, ConflictBehavior Behavior, params object[] tail)
        {
            var headMap = head.ToAttributeDictionary();
            var tailMaps = tail.ExceptWhere(x => x is null).Select(x => x.ToAttributeDictionary()).ToArray();
            var mergedMaps = headMap.MergeAttributeMaps(Behavior, tailMaps);
            object merged = mergedMaps.ToAttributeObject();
            return merged;
        }
        public static object MergeAttributes(this IEnumerable<object> objects, ConflictBehavior Behavior = ConflictBehavior.KeepLeft)
        {
            object head = objects.FirstOrDefault();
            if (head == null)
                return new { };
            return head.MergeAttributes(Behavior, objects.Skip(1).ToArray());
        }
        public static ClassSet ToClassSet(this string classTag)
        {
            if (string.IsNullOrWhiteSpace(classTag))
                return new ClassSet();
            var cs = Regex.Split(classTag, @"\s+").Select(x => x.Trim()).ToClassSet();
            return cs;
        }
        public static string ToClassTag(this ClassSet classSet)
        {
            string tag = string.Join(" ", classSet);
            return tag;
        }
        public static string MergeClasses(this string head, params string[] tail)
            => head.MergeClasses(ConflictBehavior.Overwrite, tail);
        public static string MergeClasses(this string head, ConflictBehavior behavior, params string[] tail)
        {
            var headSet = head.ToClassSet();
            var tailSets = tail.Select(x => x.ToClassSet());
            var merged = headSet.MergeClasses(behavior, tailSets);
            string tag = merged.ToClassTag();
            return tag;
        }
        public static ClassSet MergeClasses(this ClassSet set, IEnumerable<ClassSet> tailSets)
            => set.MergeClasses(ConflictBehavior.Overwrite, tailSets.ToArray());
        public static ClassSet MergeClasses(this ClassSet set, ConflictBehavior behavior, IEnumerable<ClassSet> tailSets)
            => set.MergeClasses(ConflictBehavior.Overwrite, tailSets.ToArray());
        public static ClassSet MergeClasses(this ClassSet set, params ClassSet[] tailSets)
            => set.MergeClasses(ConflictBehavior.Overwrite, tailSets);
        public static ClassSet MergeClasses(this ClassSet set, ConflictBehavior behavior, params ClassSet[] tailSets)

        {
            var merged = new ClassSet();
            merged.AddRange(set);
            foreach (var s in tailSets)
                merged.AddRange(s);
            return merged;
        }
        public static StyleMap ToStyleMap(this IEnumerable<KeyValuePair<string, string>> styles)
            => new StyleMap(styles);
        public static StyleMap ToStyleMap(this string styleTag)
        {
            if (string.IsNullOrWhiteSpace(styleTag))
                return new StyleMap();
            var assignments = styleTag.Split(';').Select(x => x.Trim()).ToList();
            var pairs = assignments
                // Split each assignment into an array of trimmed key-value strings
                .Select(x => x.Split(':').Select(y => y.Trim()).ToList())
                // Ignore empty assignments
                .Where(x => !x.Any(y => string.IsNullOrWhiteSpace(y)))
                // Ignore invalid or unparseable assignments
                .Where(x => x.Count == 2)
                // Convert arrays to KeyValuePair instances
                .Select(x => new KeyValuePair<string, string>(x[0], x[1]))
            ;
            var map = pairs.ToStyleMap();
            return map;
        }
        public static string ToStyleTag(this StyleMap styleMap)
        {
            var styleAssignments = styleMap.Select(x => string.Join(":", x.Key, x.Value)).ToList();
            string styleTag = string.Join("; ", styleAssignments);
            return styleTag;
        }

        #region MergeStyles
        public static StyleMap MergeStyles(this string head, IEnumerable<KeyValuePair<string, string>> tail)
            => ((StyleMap)head).MergeStyles((StyleMap)tail);
        public static StyleMap MergeStyles(this string head, params StyleMap[] tail)
            => ((StyleMap)head).MergeStyles(ConflictBehavior.Overwrite, tail);
        public static StyleMap MergeStyles(this string head, StyleMapCollection tail)
            => ((StyleMap)head).MergeStyles(ConflictBehavior.Overwrite, tail);
        public static StyleMap MergeStyles(this StyleMap head, params StyleMap[] tail)
            => head.MergeStyles(ConflictBehavior.Overwrite, tail);
        public static StyleMap MergeStyles(this StyleMap head, IEnumerable<KeyValuePair<string, string>> tail)
            => head.MergeStyles((StyleMap)tail);
        public static StyleMap MergeStyles(this StyleMap head, StyleMapCollection tail)
            => head.MergeStyles(ConflictBehavior.Overwrite, tail);
        private static StyleMap MergeStyles(this StyleMap map, ConflictBehavior behavior, StyleMapCollection tailMaps)
        {
            var merged = map.ToStyleMap();
            foreach (var tailMap in tailMaps)
            {
                foreach (var p in tailMap)
                {
                    switch (behavior)
                    {
                        case ConflictBehavior.Overwrite:
                            merged[p.Key] = p.Value;
                            break;
                        case ConflictBehavior.Skip:
                            if (!merged.ContainsKey(p.Key))
                                merged[p.Key] = p.Value;
                            break;
                    }
                }
            }
            return merged;
        }

        #endregion
        public static IAttrMap MergeAttributeMaps(this IAttrMap head, params IAttrMap[] tail)
            => MergeAttributeMaps(head, ConflictBehavior.KeepLeft, tail);
        public static IAttrMap MergeAttributeMaps(this IAttrMap head, ConflictBehavior Behavior, params IAttrMap[] tail)
        {
            IEnumerable<KeyValuePair<string, object>> concat = head;
            foreach (var t in tail)
                concat = concat.Concat(t);
            var merged = new Dictionary<string, object>();
            foreach (var pair in concat)
            {
                string key = pair.Key.ToLower();
                if (merged.ContainsKey(key))
                {
                    if (key == "style")
                    {
                        string mergedStyle = merged[key].ToString();
                        mergedStyle = pair.Value?.ToString().MergeStyles(mergedStyle);
                        merged[key] = mergedStyle;
                    }
                    else if (key == "class")
                    {
                        string mergedClasses = merged[key].ToString();
                        mergedClasses = mergedClasses.MergeClasses(pair.Value?.ToString());
                        merged[key] = mergedClasses;
                    }
                }
                else
                {
                    merged[pair.Key] = pair.Value;
                }
            }
            return merged;
        }
        public static IAttrMap MergeAttributeMaps(this IEnumerable<IAttrMap> maps)
        {
            var head = maps.FirstOrDefault();
            if (head == null)
                return new Dictionary<string, object>();
            return head.MergeAttributeMaps(maps.Skip(1).ToArray());
        }
        public static object ToAttributeObject(this IAttrMap dict)
        {
            var anonObject = new ExpandoObject();
            var expandoDictionary = (IDictionary<string, object>)anonObject;
            var keys = new HashSet<string>(dict.Keys, StringComparer.OrdinalIgnoreCase);
            foreach (string key in keys)
                expandoDictionary[key] = dict[key];
            return anonObject;
        }
        public static Dictionary<string, object> ToAttributeDictionary(this object values)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (values == null)
                return dict;
            var cast = values as IDictionary<string, object>;
            if (cast != null)
                return cast.ToDictionary(x => x.Key, x => x.Value);
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
            {
                object obj = propertyDescriptor.GetValue(values);
                dict.Add(propertyDescriptor.Name, obj);
            }
            return dict;
        }

        public static string FormatHtml(this string html)
        {
            string repaired, formatted;
            // Add any missing tags
            using (var swriter = new StringWriter())
            {
                var hdoc = new HtmlDocument();
                hdoc.LoadHtml(html);
                using (var xwriter = new XmlTextWriter(swriter))
                {
                    xwriter.Formatting = Formatting.Indented;
                    xwriter.Indentation = 4;
                    hdoc.Save(xwriter);
                }
                repaired = swriter.ToString();
            }
            // Format with correct indentation
            using (var swriter = new StringWriter())
            {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(repaired);
                using (var xwriter = new XmlTextWriter(swriter))
                {
                    xwriter.Formatting = Formatting.Indented;
                    xwriter.Indentation = 4;
                    xdoc.Save(xwriter);
                }
                formatted = swriter.ToString();
            }
            return formatted;
        }

        public static string ToHtmlString(this IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                string html = writer.ToString();
                return html;
            }
        }
    }
}
