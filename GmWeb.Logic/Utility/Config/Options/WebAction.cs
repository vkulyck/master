using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using GmWeb.Logic.Utility.Extensions.Http;

namespace GmWeb.Logic.Utility.Config;

public class WebAction
{
    [JsonIgnore]
    public WebAppOptions Tier { get; set; }
    public string Path { get; set; }
    public string AppUrl => this.Tier?.AppUrl;
    public string Url
    {
        get
        {
            if (this.Tier is null)
                throw new ArgumentNullException(nameof(this.Tier));

            return $"{this.AppUrl}/{this.Path}";
        }
    }
    public WebAction() : this(default(string), default(WebAppOptions)) { }
    public WebAction(string path) : this(path, default(WebAppOptions)) { }

    public WebAction(WebAction other) : this(other.Path, other.Tier) { }
    public WebAction(Uri uri, WebAppOptions tier) : this(uri.PathAndQuery, tier) { }
    public WebAction(string path, WebAppOptions tier)
    {
        if(path is not null)
            this.Path = Regex.Replace(path, "^/+", "");
        this.Tier = tier;
    }

    public override string ToString() => this.Path;

    public WebAction WithQuery(string name, string value)
    {
        var url = this.Url.WithQuery(name, value);
        var action = new WebAction(url, this.Tier);
        return action;
    }
    public WebAction WithQuery(object data)
    {
        var url = this.Url.WithQuery(data);
        var action = new WebAction(url, this.Tier);
        return action;
    }
}
