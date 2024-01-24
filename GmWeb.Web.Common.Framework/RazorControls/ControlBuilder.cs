using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Reflection;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Web.Common.RazorControls.ControlBuilders.Specialized;

namespace GmWeb.Web.Common.RazorControls
{
    public enum HorizontalAlignment
    {
        Left,
        Right
    }

    public class ControlBuilder : ControlBuilder<ControlBuilder>
    {
        public ControlBuilder(string tag) : base(tag)
        {

        }
    }
    public abstract class HtmlControlBuilder<TBuilder> : ControlBuilder<TBuilder> where TBuilder : HtmlControlBuilder<TBuilder>
    {
        protected HtmlHelper Html { get; private set; }
        public HtmlControlBuilder(HtmlHelper html, string TagName) : base(TagName) 
        {
            this.Html = html;
        }
    }

    public abstract class ControlBuilder<TBuilder> : ControlBuilderBase where TBuilder : ControlBuilder<TBuilder>
    {
        public ControlBuilder(string TagName) : base(TagName) { }
        public virtual TBuilder Name(string name)
        {
            this.GenerateId(name);
            return (TBuilder)this;
        }

        protected TBuilder ResetChildren()
        {
            this.ChildContainer.Clear();
            return (TBuilder)this;
        }

        public virtual TBuilder Enable(bool enable)
        {
            string key = "disabled", value = "disabled";
            var exists = this.Attributes.ContainsKey(key);
            if (enable && exists)
                this.Attributes.Remove(key);
            else if (enable && !exists) { } // Do nothing
            else // Always just manually disable if disabling is requested
            {
                this.Attributes[key] = value;
                this.Style("pointer-events: none; opacity: 0.4;");
            }
            this.Enabled = enable;
            return (TBuilder)this;
        }

        public virtual TBuilder Enable() => Enable(true);
        public virtual TBuilder Disable() => Enable(false);

        public TBuilder HtmlAttributes(object attributes)
        {
            if (attributes == null)
                return (TBuilder)this;
            var dict = attributes as IDictionary<string, object>;
            if (dict != null)
                return this.HtmlAttributes(dict);
            var properties = attributes.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var key = prop.Name.ToLowerInvariant();
                string incomingValue = prop.GetValue(attributes)?.ToString();
                string currentValue = null;
                if(this.Attributes.ContainsKey(key))
                    currentValue = this.Attributes[key];
                if (string.IsNullOrWhiteSpace(currentValue))
                {
                    this.Attributes[key] = incomingValue;
                    continue;
                }
                string newValue;
                switch(key)
                {
                    case "style":
                        newValue = currentValue.MergeStyles(incomingValue);
                        break;
                    case "class":
                        newValue = currentValue.MergeClasses(incomingValue);
                        break;
                    default:
                        newValue = incomingValue;
                        break;

                }
                this.Attributes[key] = newValue;
            }
            return (TBuilder)this;
        }

        public TBuilder HtmlAttributes(IEnumerable<KeyValuePair<string, object>> attributeValues)
        {
            var dict = attributeValues.ToDictionary(x => x.Key, x => x.Value);
            return this.HtmlAttributes(dict);
        }
        public TBuilder HtmlAttributes(params KeyValuePair<string, object>[] attributeValues)
            => this.HtmlAttributes((IEnumerable<KeyValuePair<string, object>>)attributeValues);

        public TBuilder HtmlAttributes(IDictionary<string,object> attributes)
        {
            if (attributes == null)
                return (TBuilder)this;
            foreach(var key in attributes.Keys)
            {
                var value = attributes[key];
                this.Attributes[key] = value?.ToString();
            }
            return (TBuilder)this;
        }

        public virtual TBuilder DataBind(string field)
        {
            this.Attributes["data-bind"] = $"value: {field}";
            return (TBuilder)this;
        }

        public virtual TBuilder Content(string content)
        {
            this.InnerHtml = content;
            this.ChildContainer.Clear();
            return (TBuilder)this;
        }

        public TBuilder Style(string style)
        {
            var current = this.StyleTag;
            var updated = this.StyleTag.MergeStyles(style);
            this.Attributes["style"] = updated;
            return (TBuilder)this;
        }

        public TBuilder Style(params StylePair[] styles)
            => this.Style(styles.Cast<KeyValuePair<string,string>>());
        public TBuilder Style(IEnumerable<KeyValuePair<string, string>> styles)
            => this.Style((StyleMap)styles);

        public TBuilder Style(StyleMap styles)
        {
            var merged = this.StyleTag.MergeStyles(styles);
            this.Attributes["style"] = merged;
            return (TBuilder)this;
        }

        public virtual TBuilder Align(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.Right)
                this.Style("text-align: right; padding-right: 5px;");
            return (TBuilder)this;
        }

        public T CreateChild<T>(int? index = null) where T : IHtmlString, new()
        {
            var child = new T();
            this.InsertChild(child, index);
            return child;
        }

        public MvcHtmlString CreateChild(MvcHtmlString child, int? index = null)
        {
            this.InsertChild(child, index);
            return child;
        }

        public ControlBuilder CreateChild(string tag, int? index = null)
        {
            var child = new ControlBuilder(tag);
            this.InsertChild(child, index);
            return child;
        }
        public TBuilder AppendChild<T>(T Child) where T : IHtmlString
            => this.InsertChild(Child);
        public TBuilder PrependChild<T>(T Child) where T : IHtmlString
            => this.InsertChild(Child, index: 0);
        public TBuilder InsertChild<T>(T child, int? index = null) where T : IHtmlString
        {
            if (index == null)
                index = this.ChildContainer.Count;
            this.ChildContainer.Insert(index.Value, child);
            this.InnerHtml = null;
            return (TBuilder)this;
        }

        public TextBuilder CreateText(string text, int? index = null)
        {
            var child = new TextBuilder(text);
            this.InsertChild(child, index);
            return child;
        }

        public new TBuilder AddCssClass(string cls)
            => this.AddCssClass(new string[] { cls });

        public TBuilder AddCssClass(params string[] classes)
        {
            foreach(var cls in classes)
            {
                base.AddCssClass(cls);
            }
            return (TBuilder)this;
        }
    }
}
