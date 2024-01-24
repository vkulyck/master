using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Logic.Utility.Primitives;

public static class ViewerFilter
{
    public static ViewerFilter<TEnum> Where<TEnum>(TEnum filter)
        where TEnum : struct, Enum
        => new ViewerFilter<TEnum> { Include = filter };
    public static ViewerFilter<TEnum> Except<TEnum>(TEnum filter)
        where TEnum : struct, Enum
        => new ViewerFilter<TEnum> { Exclude = filter };
}

public record class ViewerFilter<TEnum> where TEnum : struct, Enum
{
    public TEnum Include { get; set; }
    public TEnum Exclude { get; set; }
}
