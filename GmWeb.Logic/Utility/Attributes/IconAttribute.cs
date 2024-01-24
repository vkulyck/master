using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Utility.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class IconAttribute : Attribute
{
    public string IconClass => $"{this.Prefix} fa-{this.IconName}";
    public string IconName { get; protected set; }
    public bool IsSolid { get; set; }
    protected string Prefix => this.IsSolid ? "fas" : "fa";
    public IconAttribute(string IconName, bool IsSolid = true)
    {
        this.IconName = IconName;
        this.IsSolid = IsSolid;
    }
}
