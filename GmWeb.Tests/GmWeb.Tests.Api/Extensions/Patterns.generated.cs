using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GmWeb.Tests.Api.Extensions;

public static partial class Patterns
{
    public static readonly Regex AnyNumeric = new Regex(@"[0-9]");
    public static readonly Regex AnyNonNumeric = new Regex(@"[^0-9]");
    public static readonly Regex OnlyNumeric = new Regex(@"^[0-9]+$");
    public static readonly Regex OnlyNonNumeric = new Regex(@"^[^0-9]+$");
    public static readonly Regex SeqNumeric = new Regex(@"[0-9]+");
    public static readonly Regex AnyUpper = new Regex(@"[A-Z]");
    public static readonly Regex AnyNonUpper = new Regex(@"[^A-Z]");
    public static readonly Regex OnlyUpper = new Regex(@"^[A-Z]+$");
    public static readonly Regex OnlyNonUpper = new Regex(@"^[^A-Z]+$");
    public static readonly Regex SeqUpper = new Regex(@"[A-Z]+");
    public static readonly Regex AnyLower = new Regex(@"[a-z]");
    public static readonly Regex AnyNonLower = new Regex(@"[^a-z]");
    public static readonly Regex OnlyLower = new Regex(@"^[a-z]+$");
    public static readonly Regex OnlyNonLower = new Regex(@"^[^a-z]+$");
    public static readonly Regex SeqLower = new Regex(@"[a-z]+");
    public static readonly Regex AnyAlpha = new Regex(@"[A-Za-z]");
    public static readonly Regex AnyNonAlpha = new Regex(@"[^A-Za-z]");
    public static readonly Regex OnlyAlpha = new Regex(@"^[A-Za-z]+$");
    public static readonly Regex OnlyNonAlpha = new Regex(@"^[^A-Za-z]+$");
    public static readonly Regex SeqAlpha = new Regex(@"[A-Za-z]+");
    public static readonly Regex AnySpacing = new Regex(@"[\s]");
    public static readonly Regex AnyNonSpacing = new Regex(@"[^\s]");
    public static readonly Regex OnlySpacing = new Regex(@"^[\s]+$");
    public static readonly Regex OnlyNonSpacing = new Regex(@"^[^\s]+$");
    public static readonly Regex SeqSpacing = new Regex(@"[\s]+");
}
