using System;
using System.Collections.Generic;
using System.Linq;
using GmWeb.Logic.Utility.Extensions.Enums;
using GmWeb.Logic.Utility.Attributes;

namespace GmWeb.Web.RHI.Utility;

public record class IntakePage : EnumViewModel<IntakePageType>
{
    private static readonly List<IntakePage> _pages = EnumExtensions.GetEnumViewModels<IntakePageType, IntakePage>();
    public static IReadOnlyList<IntakePage> GetPages() => _pages;
    public static IntakePage GetPage(IntakePageType pageType) => _pages[(int)pageType];
    public IntakePage(int pageID) : base(pageID) { }
    public IntakePage(IntakePageType pageType) : base(pageType) { }

    public int Number => this.ID + 1;
    public string TabIcon => this.GetAttribute<IconAttribute>()?.IconClass;
    public string CacheGotoInvocation => $"Navigator.cacheGoto('{this.Name}')";
    public string TabViewPath => $"/Views/Intake/{this.Name}.cshtml";
    public string NavPagerID => $"NavPager-{this.Name}";
    public string NavMenuID => $"NavMenu-{this.Name}";
    public string TabID => $"Tab-{this.Name}";
    public string FormID => $"Form-{this.Name}";
    public string NewJsInstance => $@"new {this.GetType().Name}({{
        name: '{this.Name}',
        index: {this.ID},
        navPagerID: '{this.NavPagerID}',
        navMenuID: '{this.NavMenuID}',
        tabID: '{this.TabID}',
        formID: '{this.FormID}'
    }})";
}