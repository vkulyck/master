using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Utility.Extensions
{
    public static class ParsingExtensions
    {
        public static string ReplaceGroup(this Regex regex, string input, string groupName, string replacement)
        {
            string output = input;
            int startIndex = 0;
            while (true)
            {
                var match = regex.Match(output, startIndex);
                if (!match.Success)
                    break;
                var group = match.Groups[groupName].Captures[0];
                string start = output.Substring(0, group.Index);
                string middle = replacement;
                string end = output.Substring(group.Index + group.Length, output.Length - (group.Index + group.Length));
                output = start + middle + end;
                startIndex = start.Length + middle.Length;
            }
            return output;
        }

        public static bool TryMatch(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }

        public static decimal GetCurrency(this Match match, string group)
        {
            var style = System.Globalization.NumberStyles.Currency;
            var format = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string amountStr = match.Groups[group].Value;
            if (decimal.TryParse(amountStr, style, format, out decimal amount))
            {
                return amount;
            }
            throw new Exception($"Currency amount could not be parsed from match string: {amountStr}");
        }

        public static List<decimal> GetCurrencies(this Match match, string group)
        {
            var style = System.Globalization.NumberStyles.Currency;
            var format = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            var currencies = new List<decimal>();
            foreach (Capture capture in match.Groups[group].Captures)
            {
                if (decimal.TryParse(capture.Value, style, format, out decimal amount))
                    currencies.Add(amount);
                else
                    throw new Exception($"Error parsing currency string: {capture.Value}");
            }
            return currencies;
        }
    }
}
