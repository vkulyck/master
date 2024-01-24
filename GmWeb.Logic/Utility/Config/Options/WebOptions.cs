using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using GmWeb.Logic.Utility.Extensions.Reflection;

namespace GmWeb.Logic.Utility.Config;

public class WebOptions
{
    public string BaseDomain { get; set; }
    public string CommonCookieDomain => $".{BaseDomain}";
    public string DataProtectionKeyDirectory { get; set; }

    protected void Initialize<TWeb>(TWeb Tier)
        where TWeb : WebAppOptions, new()
    {
        var tiers = this.GetPropertyValues<WebAppOptions>();
        foreach (var tier in tiers)
            tier.Env = this;
        var actions = this.GetPropertyValues<WebAction>();
        foreach (var action in actions)
            action.Tier = Tier;
    }
}
