using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions.Expressions;
using GmWeb.Logic.Utility.Extensions.Html;

namespace GmWeb.Web.RHI.Views.Shared.Component;

public static class ComponentExtensions
{
    public static async Task<IHtmlContent> BitVectorControlFor<TModel, TProperty>(this IHtmlHelper<TModel> html, TModel model, Expression<Func<TModel, TProperty?>> propertySelector)
        where TProperty : struct, Enum
        => await BitVectorControlFor(html, model, propertySelector, attributes: null);
    public static async Task<IHtmlContent> BitVectorControlFor<TModel, TProperty>(this IHtmlHelper<TModel> html, TModel model, Expression<Func<TModel, TProperty?>> propertySelector, object attributes)
        where TProperty : struct, Enum
    {
        var select = propertySelector.Compile();
        var propertyName = propertySelector.GetPropertyName();
        var componentAttributes = new { InputSelection = select(model), ID = propertyName }.MergeAttributes(attributes);
        var controlContent = await html.RenderComponentAsync<BitVectorControl<TProperty>>(RenderMode.ServerPrerendered, componentAttributes);
        return controlContent;
    }
    public static async Task<IHtmlContent> BitVectorControlFor<TModel, TProperty>(this IHtmlHelper<TModel> html, TModel model, Expression<Func<TModel, TProperty>> propertySelector)
        where TProperty : struct, Enum
        => await BitVectorControlFor(html, model, propertySelector, attributes: null);
    public static async Task<IHtmlContent> BitVectorControlFor<TModel,TProperty>(this IHtmlHelper<TModel> html, TModel model, Expression<Func<TModel,TProperty>> propertySelector, object attributes)
        where TProperty : struct, Enum
    {
        var select = propertySelector.Compile();
        var propertyName = propertySelector.GetPropertyName();
        var componentAttributes = new { InputSelection = select(model), ID = propertyName }.MergeAttributes(attributes);
        var controlContent = await html.RenderComponentAsync<BitVectorControl<TProperty>>(RenderMode.ServerPrerendered, componentAttributes);
        return controlContent;
    }
}