using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Common.RazorControls;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Common
{
    public static class RazorExtensions
    {
        public static GmFactory GmControls(this HtmlHelper helper)
        {
            var factory = new GmFactory(helper);
            return factory;
        }
        public static GmModelFactory<T> GmRazor<T>(this HtmlHelper<T> helper) where T : class, new()
        {
            var factory = new GmModelFactory<T>(helper);
            return factory;
        }

        // TODO: Use Kendo for now, but if necessary then use a GmFactory with custom controls
        public static GmKendoFactory Razor(this HtmlHelper helper)
        {
            var factory = new GmKendoFactory(helper);
            return factory;
        }

        public static Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U> MergeHtmlAttributes<T, U>(
                this Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U> builder,
                object attributes
            ) where T : Kendo.Mvc.UI.WidgetBase where U : Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U>
        {
            var attDict = attributes.ToDictionary();
            return builder.MergeHtmlAttributes(attDict);
        }

        public static Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U> MergeHtmlAttributes<T, U>(
                this Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U> builder,
                IDictionary<string,object> attributes
            ) where T : Kendo.Mvc.UI.WidgetBase where U : Kendo.Mvc.UI.Fluent.WidgetBuilderBase<T, U>
        {
            var builderAttributes = builder.ToComponent().HtmlAttributes.ToDictionary(x => x.Key, x => x.Value);
            var merged = builderAttributes.MergeLeft(attributes);

            // Concatenate style
            string style = string.Empty;
            if (builderAttributes.TryGetValue("style", out object o1))
                style += o1?.ToString() ?? string.Empty;
            if(attributes.TryGetValue("style", out object o2))
                style += o2?.ToString() ?? string.Empty;
            if (style != string.Empty)
                merged["style"] = style;

            // Concatenate classes
            string @class = string.Empty;
            if (builderAttributes.TryGetValue("class", out object o3))
                @class += o3?.ToString() ?? string.Empty;
            if (attributes.TryGetValue("class", out object o4))
                @class += o4?.ToString() ?? string.Empty;
            if (@class != string.Empty)
                merged["class"] =  @class;

            var ss = merged["style"]?.ToString();
            var cc = merged["class"]?.ToString();
            builder.HtmlAttributes(merged);
            return builder;
        }

        public static EditorType ToEditorType(this TypeCode tc)
        {
            switch (tc)
            {
                case TypeCode.Boolean:
                    return EditorType.CheckBox;
                case TypeCode.Char:
                    return EditorType.CharBox;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return EditorType.SignedIntBox;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return EditorType.UnsignedIntBox;
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return EditorType.FloatBox;
                case TypeCode.DateTime:
                    return EditorType.CalendarClock;
                case TypeCode.String:
                    return EditorType.TextBox;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.Object:
                default:
                    return EditorType.Undefined;
            }
        }
    }
}