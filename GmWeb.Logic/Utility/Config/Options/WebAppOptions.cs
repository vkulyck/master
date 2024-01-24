using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using GmWeb.Logic.Utility.Extensions.Collections;

namespace GmWeb.Logic.Utility.Config;
public class WebAppOptions
{
    [JsonIgnore]
    public WebOptions Env { get; set; }
    private string _name;
    public string Name
    {
        get => this._name ?? this._sub;
        set => this._name = value;
    }
    private string _sub;
    public string Subdomain
    {
        get => this._sub ?? this._name?.ToLower();
        set => this._sub = value;
    }
    public string PathPrefix { get; set; } = string.Empty;
    public string Scheme { get; set; } = "https:";
    public Uri BaseUri => new Uri(this.AppUrl + "/");
    public string AppUrl
    {
        get
        {
            string domain = ".".JoinNonNull(this.Subdomain, this.Env?.BaseDomain);
            string combined = $"{this.Scheme}//{domain}/{this.PathPrefix}";
            string trimmed = Regex.Replace(combined, @"/+$", string.Empty);
            return trimmed;
        }
    }

    public WebAppOptions() { }
    public WebAppOptions(string Name)
    {
        this.Name = Name;
    }
    public WebAppOptions(string Name, string PathPrefix)
        : this(Name)
    {
        this.PathPrefix = PathPrefix;
    }

    public static implicit operator string(WebAppOptions app) => app.AppUrl;
    public static implicit operator Uri(WebAppOptions app) => new Uri(app.AppUrl);
}