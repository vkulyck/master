using System.Text.RegularExpressions;
using BaseAssert = Xunit.Assert;

namespace GmWeb.Tests.Api.Extensions;

public class GmAssert : BaseAssert
{
    public static class Math
    {
        public static void ExceedsRatio(int numerator, int denominator, decimal ratio)
            => Assert.True(numerator > ratio * denominator);
    }
    public static class String
    {
        public static void IsUpperCase(string value)
        {
            var glyphs = Patterns.SeqSpacing.Replace(value, string.Empty);
            DoesNotMatch(Patterns.AnyLower, glyphs);
            Math.ExceedsRatio(Patterns.AnyUpper.Matches(glyphs).Count, Patterns.AnyNonUpper.Matches(glyphs).Count, 3M);
        }

        public static void IsLowerCase(string value)
        {
            var glyphs = Patterns.SeqSpacing.Replace(value, string.Empty);
            DoesNotMatch(Patterns.AnyUpper, glyphs);
            Math.ExceedsRatio(Patterns.AnyLower.Matches(glyphs).Count, Patterns.AnyNonLower.Matches(glyphs).Count, 3M);
        }
    }
}
