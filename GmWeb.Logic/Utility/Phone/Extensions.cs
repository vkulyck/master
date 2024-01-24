using System;
using System.ComponentModel;

namespace GmWeb.Logic.Utility.Phone
{
    internal static class Extensions
    {

        public static bool IsNullOrEmpty(this string pString) => string.IsNullOrEmpty(pString);

        public static bool IsNumber(this string pString)
        {
            bool isNumeric = long.TryParse(pString, out long n);
            return isNumeric;
        }
        public static string GetDescription(this Enum enumObj)
        {
            if (enumObj == null)
                return string.Empty;

            var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            for (int i = 0; i < attribArray.Length; i++)
            {
                if (attribArray[i].ToString().Contains("Description"))
                    return ((DescriptionAttribute)attribArray[i]).Description;
            }
            return string.Empty;
        }

    }
}
