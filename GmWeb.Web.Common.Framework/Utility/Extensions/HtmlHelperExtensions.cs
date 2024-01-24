using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Interfaces;
using System.Linq.Expressions;
using System.Reflection;
using GmWeb.Logic.Enums;
using GmWeb.Web.Common.Models;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.Models.Shared;

namespace GmWeb.Web.Common.Utility
{
    public static class HtmlHelperExtensions
    {
        public static bool IsDebugBuild(this HtmlHelper html)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
        public static bool IsSecurityBypassBuild(this HtmlHelper html)
        {
#if SECURITY_BYPASS
            return true;
#else
            return false;
#endif
        }
        public static bool IsAutologinBuild(this HtmlHelper html)
        {
#if AUTOLOGIN
            return true;
#else
            return false;
#endif
        }
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string CurrentViewName(this HtmlHelper html)
        {
            var webPage = html.ViewDataContainer as System.Web.WebPages.WebPageBase;
            var virtualPath = webPage.VirtualPath;
            var viewName = System.IO.Path.GetFileNameWithoutExtension(virtualPath);
            return viewName;
        }

        public static IDisposable DelayedHandlers(this HtmlHelper html)
        {
            var viewName = html.CurrentViewName();
            return html.Delayed(isOnlyOne: $"{viewName}:Handlers");
        }

        public static IHtmlString ConvertInstanceToJs<T>(this HtmlHelper html, T instance) where T : IViewModel, new()
        {
            var serialized = SerializationExtensions.Serialize(instance);
            var raw = html.Raw(serialized);
            return raw;
        }

        public static IHtmlString RawViewDataAttributes(this HtmlHelper html)
        {
            dynamic attributes = html.ViewData["attr"];
            if (attributes == null)
                return html.Raw("");
            var dict = new RouteValueDictionary(attributes);
            var pairs = dict.Select(x => $"{x.Key.Replace("_","-")}='{x.Value}'").ToList();
            var full = string.Join(" ", pairs);
            return html.Raw(full);
        }

        public static string GenerateGuid(this HtmlHelper html) => ModelExtensions.GenerateGuid();


        #region Editors
        public static IHtmlString CollectionEditorFor<TModel, TItem>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, IEnumerable<TItem>>> expression,
            string CollectionTitle = null,
            string ItemTitle = null,
            bool StartCollapsed = true,
            bool AreItemsCollapsible = false,
            bool StartItemsCollapsed = false
        )
            where TModel : IEditableViewModel
            where TItem : IEditableViewModel
        {
            TModel model = html.ViewData.Model;
            var property = expression.GetSelectorProperty();
            var ParentGuid = model.Guid;
            var ItemTypeName = typeof(TItem).Name;
            var CollectionName = property.Name;
            var data = new { CollectionTitle, ItemTitle, ParentGuid, CollectionName, ItemTypeName, StartCollapsed, AreItemsCollapsible, StartItemsCollapsed };
            return html.EditorFor(expression, "CollectionContainer", data);
        }

        public static IHtmlString CollectionItemEditorFor<TItem>(this HtmlHelper html, TItem item)
            where TItem : IEditableViewModel
        => html.CollectionItemEditorFor(item, null);

        public static IHtmlString CollectionItemEditorFor<TItem>(
            this HtmlHelper html,
            TItem item,
            object additionalViewData
        )
            where TItem : IEditableViewModel
        {
            var dict = additionalViewData?.ToViewDataDictionary() ?? new ViewDataDictionary();
            return html.Partial("EditorTemplates/CollectionItemContainer", item, dict);
        }

        public static IHtmlString EditorContainerFor<TModel, TItem>(this HtmlHelper<TModel> html, Expression<Func<TModel, TItem>> expression)
            where TItem : IEditableViewModel
        => html.EditorContainerFor(expression, null);

        public static IHtmlString EditorContainerFor<TModel, TItem>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TItem>> expression,
            object additionalViewData
        ) where TItem : IEditableViewModel
        {
            var editor = html.EditorFor(expression, "FieldContainer", additionalViewData);
            return editor;
        }
        public static IHtmlString EditorContainerForModel<TModel>(this HtmlHelper<TModel> html)
            where TModel : IEditableViewModel
        => html.EditorContainerForModel(null);

        public static IHtmlString EditorContainerForModel<TModel>(
            this HtmlHelper<TModel> html,
            object additionalViewData
        ) where TModel : IEditableViewModel
        {
            var template = $"EditorTemplates/FieldContainer";
            var dict = additionalViewData?.ToViewDataDictionary() ?? new ViewDataDictionary();
            return html.Partial(template, html.ViewData.Model, dict);
        }

        public static MvcHtmlString EditorForModel<T>(this HtmlHelper<T> html)
        {
            var model = html.ViewData.Model;
            var template = $"EditorTemplates/{model.GetType().Name}";
            return html.Partial(template, model);
        }
        #endregion

        #region Displays
        public static IHtmlString CollectionDisplayFor<TModel, TItem>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, IEnumerable<TItem>>> expression,
            string CollectionTitle = null
        )
            where TModel : IViewModel
            where TItem : IViewModel
        {
            TModel model = html.ViewData.Model;
            var property = expression.GetSelectorProperty();
            var ParentGuid = model.Guid;
            var ItemTypeName = typeof(TItem).Name;
            var CollectionName = property.Name;
            var data = new { CollectionTitle, ParentGuid, CollectionName, ItemTypeName };
            return html.DisplayFor(expression, "CollectionContainer", data);
        }

        public static IHtmlString CollectionItemDisplayFor<TItem>(this HtmlHelper html, TItem item)
            where TItem : IViewModel
        => html.CollectionItemDisplayFor(item, null);

        public static IHtmlString CollectionItemDisplayFor<TItem>(
            this HtmlHelper html,
            TItem item,
            object additionalViewData
        )
            where TItem : IViewModel
        {
            var dict = additionalViewData?.ToViewDataDictionary() ?? new ViewDataDictionary();
            return html.Partial("DisplayTemplates/CollectionItemContainer", item, dict);
        }

        public static IHtmlString DisplayContainerFor<TModel, TItem>(this HtmlHelper<TModel> html, Expression<Func<TModel, TItem>> expression)
            where TItem : IViewModel
        => html.DisplayContainerFor(expression, null);

        public static IHtmlString DisplayContainerFor<TModel, TItem>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TItem>> expression,
            object additionalViewData
        ) where TItem : IViewModel
        {
            var editor = html.DisplayFor(expression, "FieldContainer", additionalViewData);
            return editor;
        }
        public static IHtmlString DisplayContainerForModel<TModel>(this HtmlHelper<TModel> html)
            where TModel : IViewModel
        => html.DisplayContainerForModel(null);

        public static IHtmlString DisplayContainerForModel<TModel>(
            this HtmlHelper<TModel> html,
            object additionalViewData
        ) where TModel : IViewModel
        {
            var template = $"DisplayTemplates/FieldContainer";
            var dict = additionalViewData?.ToViewDataDictionary() ?? new ViewDataDictionary();
            return html.Partial(template, html.ViewData.Model, dict);
        }

        public static MvcHtmlString DisplayForModel<T>(this HtmlHelper<T> html)
        {
            var model = html.ViewData.Model;
            var template = $"DisplayTemplates/{model.GetType().Name}";
            return html.Partial(template, model);
        }
        #endregion

        public static HtmlHelper<TModel> For<TModel>(this HtmlHelper helper) where TModel : class, new()
        {
            return For<TModel>(helper.ViewContext, helper.ViewDataContainer.ViewData, helper.RouteCollection);
        }

        public static HtmlHelper<TModel> For<TModel>(this HtmlHelper helper, TModel model)
        {
            return For<TModel>(helper.ViewContext, helper.ViewDataContainer.ViewData, helper.RouteCollection, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection) where TModel : class, new()
        {
            TModel model = new TModel();
            return For<TModel>(viewContext, viewData, routeCollection, model);
        }

        public static HtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, RouteCollection routeCollection, TModel model)
        {
            var newViewData = new ViewDataDictionary(viewData) { Model = model };
            ViewContext newViewContext = new ViewContext(
                viewContext.Controller.ControllerContext,
                viewContext.View,
                newViewData,
                viewContext.TempData,
                viewContext.Writer);
            var viewDataContainer = new ViewDataContainer(newViewContext.ViewData);
            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, routeCollection);
        }

        private class ViewDataContainer : System.Web.Mvc.IViewDataContainer
        {
            public System.Web.Mvc.ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer(System.Web.Mvc.ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }
        }

        public static MvcHtmlString CreateDefaultEditor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector
        ) where TModel : class, new()
        => html.CreateDefaultEditor(fieldSelector, null, EditorType.Undefined);
        public static MvcHtmlString CreateDefaultEditor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            EditorType editorType
        ) where TModel : class, new()
        => html.CreateDefaultEditor(fieldSelector, null, editorType);
        public static MvcHtmlString CreateDefaultEditor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            object htmlAttributes
        ) where TModel : class, new()
        => html.CreateDefaultEditor(fieldSelector, htmlAttributes, EditorType.Undefined);
        public static MvcHtmlString CreateDefaultEditor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> fieldSelector,
            object htmlAttributes,
            EditorType editorType
        ) where TModel : class, new()
        {
            IDictionary<string,object> attributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
                attributes = htmlAttributes.ToAttributeDictionary();
            var rzr = html.GmRazor();
            if (editorType == EditorType.Hidden)
                return html.HiddenFor(fieldSelector);
            switch (fieldSelector)
            {
                case Expression<Func<TModel, bool>> checkboxSelector:
                    return rzr.CheckBoxFor(checkboxSelector, attributes);
                case Expression<Func<TModel, int>> intSelector:
                    return rzr.NumericTextBoxFor(intSelector, attributes);
                case Expression<Func<TModel, float>> floatSelector:
                    return rzr.FloatTextBoxFor(floatSelector, attributes);
                case Expression<Func<TModel, double>> doubleSelector:
                    return rzr.FloatTextBoxFor(doubleSelector, attributes);
                case Expression<Func<TModel, decimal>> decimalSelector:
                    return rzr.FloatTextBoxFor(decimalSelector, attributes);
                case Expression<Func<TModel, DateTime>> datetimeSelector:
                    return rzr.CalendarClockFor(datetimeSelector, attributes);
                case Expression<Func<TModel, string>> stringSelector:
                    switch (editorType)
                    {
                        default:
                            return rzr.TextBoxFor(stringSelector, attributes);
                        case EditorType.TextArea:
                            return rzr.TextAreaFor(stringSelector, attributes);
                        case EditorType.PasswordBox:
                            return rzr.PasswordBoxFor(stringSelector, attributes);
                    }
                default:
                    throw new NotImplementedException();

            }
        }
    }
}
