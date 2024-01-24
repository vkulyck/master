using System;
using GmWeb.Logic.Utility.Extensions.Enums;

namespace GmWeb.Web.RHI.Views.Shared.Component;

public class EnumCheckBox<TEnum>
    where TEnum : struct, Enum
{
    public TEnum Flag { get; }
    public int Value { get; }
    public bool IsSelected { get; set; } = false;

    public EnumCheckBox(TEnum flag)
    {
        this.Flag = flag;
        this.Value = this.Flag.ToN();
    }
}
