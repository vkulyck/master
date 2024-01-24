using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Logic.Utility.Extensions.Enums;
using Kendo.Mvc.UI.Fluent;
using GmWeb.Logic.Services.Datasets.Countries;
using GmWeb.Logic.Services.Datasets.Languages;
using GmWeb.Logic.Services.Datasets.Ethnicities;
using Algenta.Globalization.LanguageTags;

namespace GmWeb.Web.RHI.Utility;

public static class Extensions
{
    public static bool IsDevelopment<T>(this IHtmlHelper<T> html)
    {
        var env = html.ViewContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
        return env.IsDevelopment();
    }

    public static CheckBoxGroupBuilder HandleUpdates(this CheckBoxGroupBuilder builder, string handler)
        => builder.Events(e => e.Change(handler));
    public static RadioGroupBuilder HandleUpdates(this RadioGroupBuilder builder, string handler)
        => builder.Events(e => e.Change(handler));
    public static DropDownListBuilder HandleUpdates(this DropDownListBuilder builder, string handler)
        => builder.Events(e => e.Change(handler).DataBound(handler));
    public static MultiSelectBuilder HandleUpdates(this MultiSelectBuilder builder, string handler)
        => builder.Events(e => e.Change(handler).DataBound(handler));

    #region Enum Binding
    private static IEnumerable<SelectListItem> EnumSelectList<TEnum>()
        where TEnum : struct, Enum
        => EnumExtensions.GetEnumViewModels<TEnum>().Select(e => new SelectListItem(e.Display, e.ID.ToString()));
    private static IEnumerable<SelectListItem> EnumSelectList<TEnum, TKey>(
        Func<EnumViewModel<TEnum>, TKey> SortKeySelector,
        bool Ascending
    ) where TEnum : struct, Enum
    {
        var models = EnumExtensions
            .GetEnumViewModels<TEnum>().AsEnumerable()
        ;
        if (Ascending)
            models = models.OrderBy(SortKeySelector);
        else
            models = models.OrderByDescending(SortKeySelector);
        return models.Select(e => new SelectListItem(e.Display, e.ID.ToString()));
    }

    public static DropDownListBuilder BindToAscendingEnum<TEnum>(
        this DropDownListBuilder builder
    ) where TEnum : struct, Enum
        => builder.BindTo(EnumSelectList<TEnum,string>(e => e.Display, Ascending: true));
    public static DropDownListBuilder BindToDescendingEnum<TEnum>(
        this DropDownListBuilder builder
    ) where TEnum : struct, Enum
        => builder.BindTo(EnumSelectList<TEnum,string>(e => e.Display, Ascending: false));
    public static DropDownListBuilder BindToEnum<TEnum>(
        this DropDownListBuilder builder
    ) where TEnum : struct, Enum
        => builder.BindTo(EnumSelectList<TEnum>());
    public static RadioGroupBuilder BindToEnum<TEnum>(this RadioGroupBuilder builder)
        where TEnum : struct, Enum
        => builder.BindToKeyValues(EnumExtensions.GetEnumViewModels<TEnum>().Select(e => (e.ID, e.Display)));

    #endregion

    public static RadioGroupBuilder BindToKeyValues(this RadioGroupBuilder builder, IEnumerable<KeyValuePair<int, string>> keyValues) =>
        builder.BindToKeyValues(keyValues.Select(x => (x.Key, x.Value)));

    public static RadioGroupBuilder BindToKeyValues(this RadioGroupBuilder builder, IEnumerable<(int Value, string Display)> keyValues) =>
       builder.Items(factory => {
           foreach (var item in keyValues)
               factory.Add().Value(item.Value.ToString()).Label(item.Display);
       });

    public static DropDownListBuilder BindToLanguages(
        this DropDownListBuilder builder, 
        IHtmlHelper helper, 
        Func<LanguageService,IEnumerable<GmLanguage>> languageDatasetSelector,
        params SelectListItem[] additionalLanguages
    )
    {
        var service = helper.ViewContext.HttpContext.RequestServices.GetService<LanguageService>();
        var languageSet = languageDatasetSelector(service);
        var items = languageSet.Select(lang => new SelectListItem(text: lang.Name, value: lang.Code)).ToList();
        items.AddRange(additionalLanguages);
        return builder.BindTo(items);
    }

    public static DropDownListBuilder BindToPrimaryLanguages(this DropDownListBuilder builder, IHtmlHelper helper)
        => builder.BindToLanguages(helper, svc => svc.PrimaryLanguages, new SelectListItem { Text = "Other", Value = "other" });
    public static DropDownListBuilder BindToExtendedLanguages(this DropDownListBuilder builder, IHtmlHelper helper)
        => builder.BindToLanguages(helper, svc => svc.ExtendedLanguages);
    public static DropDownListBuilder BindToChineseLanguages(this DropDownListBuilder builder, IHtmlHelper helper)
        => builder.BindToLanguages(helper, svc => svc.ChineseLanguages);
    public static DropDownListBuilder BindToCountries(this DropDownListBuilder builder, IHtmlHelper helper)
    {
        var service = helper.ViewContext.HttpContext.RequestServices.GetService<CountryService>();
        var items = service.PrimaryCountries.Select(c => new SelectListItem(c.Name, c.CCA3));
        return builder.BindTo(items);
    }
    public static MultiSelectBuilder BindToEthnicities(this MultiSelectBuilder builder, IHtmlHelper helper)
    {
        var service = helper.ViewContext.HttpContext.RequestServices.GetService<EthnicityService>();
        var items = service.Sources.Select(c => new SelectListItem(c.Adjective, c.Code));
        return builder.BindTo(items);
    }
    public static ReadOnlyAjaxDataSourceBuilder<object> ReadAction(
        this ReadOnlyDataSourceBuilder builder, string Action, string Controller,string param =""
    ) => builder.Ajax().Read(read => read.Action(Action, Controller).Data("IncludeForgeryTokens("+ param +")"));
}
