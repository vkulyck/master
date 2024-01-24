using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Reflection;

namespace GmWeb.Web.Common.RazorControls
{
    public abstract class ControlBuilderBase : TagBuilder, IHtmlString, IDisposable
    {
        public string StyleTag => this.GetAttributeIfExists("style");
        public string ClassTag => this.GetAttributeIfExists("class");
        public bool Enabled { get; set; } = true;

        protected UrlHelper Url { get; } = new UrlHelper(HttpContext.Current.Request.RequestContext);

        protected bool HasChildren => this.ChildContainer.Count > 0;
        protected List<IHtmlString> ChildContainer { get; } = new List<IHtmlString>();
        public IEnumerable<IHtmlString> Children => this.ChildContainer;
        public virtual bool AllowSelfClosing => true;

        public ControlBuilderBase(string TagName) : base(TagName) { }

        protected string GetAttributeIfExists(string tag)
        {
            if (!this.Attributes.ContainsKey(tag))
                return null;
            return this.Attributes[tag];
        }

        protected void RemoveCssClass(string cls)
        {
            var classes = Regex.Split(this.Attributes["class"], @"\s+").ToHashSet();
            classes.Remove(cls);
            this.Attributes["class"] = string.Join(" ", classes);
        }

        protected string BuildContent()
        {
            if (this.HasChildren)
            {
                return this.BuildInnerHtml();
            }
            return this.InnerHtml;
        }

        public virtual string BuildStartHtml()
            => this.ToString(TagRenderMode.StartTag);
        public virtual string BuildInnerHtml()
        {
            using (var writer = new System.IO.StringWriter())
            {
                foreach (var child in this.ChildContainer)
                    writer.WriteLine(child.ToHtmlString());

                var html = writer.ToString();
                return html;
            }
        }
        public virtual string BuildEndHtml()
            => this.ToString(TagRenderMode.EndTag);
        public virtual string BuildChildlessHtml()
            => this.ToString(TagRenderMode.SelfClosing);

        public virtual string ToHtmlString()
        {
            if (!this.HasChildren && string.IsNullOrWhiteSpace(this.InnerHtml) && this.AllowSelfClosing)
                return this.BuildChildlessHtml();
            else if (this.HasChildren)
            {
                var start = this.BuildStartHtml();
                var middle = this.BuildInnerHtml();
                var end = this.BuildEndHtml();
                var html = $"{start}{middle}{end}";
                return html;
            }
            return this.ToString(TagRenderMode.Normal);
        }

        public virtual void Dispose() { }
    }
}