using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Web.Common.RazorControls.ControlBuilders;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common.Utility;
using System.IO;

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public abstract class EditorBuilder : IDisposable
    {
        public static EditorBuilder Instance { get; protected set; }
        public EditorOptions Options { get; protected set; }
        public EditorBuilder()
        {
            if(EditorBuilder.Instance == null)
                EditorBuilder.Instance = this;
        }
        public virtual void Dispose()
        {
            if(EditorBuilder.Instance == this)
                EditorBuilder.Instance = null;
        }
    }
    /// <summary>
    /// Builds a labeled editor for a single property of the parent page's view model.
    /// </summary>
    /// <typeparam name="TModel">The type of the page's view model.</typeparam>
    public abstract class CompositeEditorBuilder<TModel> : EditorBuilder
        where TModel : class, new()
    {
        protected List<string> GeneratedHtmlElements { get; } = new List<string>();
        protected string GeneratedHtml => string.Join("\n", this.GeneratedHtmlElements);
        public HtmlHelper<TModel> Html { get; private set; }
        private UrlHelper _Url { get; set; }
        public UrlHelper Url => this._Url;
        private string _Guid = ModelExtensions.GenerateGuid();
        public virtual string Guid => this._Guid;
        public virtual DivBuilder Container { get; protected set; } = new DivBuilder();
        public List<IHtmlString> Predecessors { get; } = new List<IHtmlString>();
        public List<IHtmlString> TopChildren { get; } = new List<IHtmlString>();
        public List<IHtmlString> BottomChildren { get; } = new List<IHtmlString>();
        public List<IHtmlString> Successors { get; } = new List<IHtmlString>();
        public CompositeEditorBuilder(HtmlHelper<TModel> html) : base()
        {
            this.Html = html;
            this._Url = new UrlHelper(this.Html.ViewContext.RequestContext, this.Html.RouteCollection);
        }

        protected virtual void BuildEditor()
        {
            this.Container = new DivBuilder();
        }


        protected TElement Prepend<TElement>()
            where TElement : IHtmlString, new()
        {
            var child = new TElement();
            this.Predecessors.Add(child);
            return child;
        }
        protected TElement PushTop<TElement>()
            where TElement : IHtmlString, new()
        {
            var child = new TElement();
            this.TopChildren.Add(child);
            return child;
        }
        protected TElement PushBottom<TElement>()
            where TElement : IHtmlString, new()
        {
            var child = new TElement();
            this.BottomChildren.Add(child);
            return child;
        }
        protected TElement Append<TElement>()
            where TElement : IHtmlString, new()
        {
            var child = new TElement();
            this.Successors.Add(child);
            return child;
        }

        protected virtual string BuildPredecessors()
        {
            using (var writer = new StringWriter())
            {
                foreach (var element in this.Predecessors)
                    writer.Write(element.ToHtmlString());
                return writer.ToString();
            }
        }
        protected virtual string BuildTopChildren()
        {
            using (var writer = new StringWriter())
            {
                foreach (var element in this.TopChildren)
                    writer.Write(element.ToHtmlString());
                return writer.ToString();
            }
        }
        protected virtual string BuildInnerEditorStart() => this.Container.BuildInnerHtml();
        protected virtual string BuildInnerEditorEnd() { return string.Empty; }
        protected virtual string BuildBottomChildren()
        {
            using (var writer = new StringWriter())
            {
                foreach (var element in this.BottomChildren)
                    writer.Write(element.ToHtmlString());
                return writer.ToString();
            }
        }
        protected virtual string BuildSuccessors()
        {
            using (var writer = new StringWriter())
            {
                foreach (var element in this.Successors)
                    writer.Write(element.ToHtmlString());
                return writer.ToString();
            }
        }

        protected virtual void WriteHtml(string html)
        {
            this.GeneratedHtmlElements.Add(html);
            this.Html.ViewContext.Writer.Write(html);
        }
        protected virtual void WriteStartHtml()
        {
            this.WriteHtml(this.BuildPredecessors());
            this.WriteHtml(this.Container.BuildStartHtml());
            this.WriteHtml(this.BuildTopChildren());
            this.WriteHtml(this.BuildInnerEditorStart());
        }
        protected virtual void WriteEndHtml()
        {
            this.WriteHtml(this.BuildInnerEditorEnd());
            this.WriteHtml(this.BuildBottomChildren());
            this.WriteHtml(this.Container.BuildEndHtml());
            this.WriteHtml(this.BuildSuccessors());
        }
    }
    public class CompositeEditorBuilder<TModel,TOptions,TBuilder> : CompositeEditorBuilder<TModel>
        where TModel : class, new()
        where TOptions : CompositeEditorOptions<TModel, TOptions>, new()
        where TBuilder : CompositeEditorBuilder<TModel, TOptions, TBuilder>
    {
        public new TOptions Options 
        { 
            get => (TOptions)base.Options;
            private set => base.Options = value;
        }
        public override string Guid => this.Options.Guid;
        public CompositeEditorBuilder(HtmlHelper<TModel> html, Action<TOptions> config)
            : base(html)
        {
            this.Options = new TOptions().WithHtml(html);
            if(config != null)
                config(this.Options);
        }

        public TBuilder BeginForm()
        {
            this.WriteStartHtml();
            return (TBuilder)this;
        }

        public override void Dispose()
        {
            this.WriteEndHtml();
            base.Dispose();
        }
    }
}