using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace GmWeb.Logic.Utility.Extensions.Enums;
public static partial class EnumExtensions
{
    public static List<TEnumViewModel> GetEnumViewModels<TEnumType, TEnumViewModel>() 
        where TEnumType : struct, Enum
        where TEnumViewModel : EnumViewModel<TEnumType>
    => GetEnumValues<TEnumType>()
        .Select(e => Activator.CreateInstance(typeof(TEnumViewModel), e))
        .Cast<TEnumViewModel>()
        .ToList()
    ;
    public static List<EnumViewModel<TEnumType>> GetEnumViewModels<TEnumType>() where TEnumType : struct, Enum
        => GetEnumValues<TEnumType>()
            .Select(x => new EnumViewModel<TEnumType>(x))
            .ToList()
    ;

    public static List<EnumViewModel<TEnumType>> GetEnumFlagPowerset<TEnumType>() where TEnumType : struct, Enum
    {
        List<EnumViewModel<TEnumType>> powerset = IterateFlagCombinations<TEnumType>()
            .Select(ev => new EnumViewModel<TEnumType>(ev))
            .ToList()
        ;
        return powerset;
    }

    // https://stackoverflow.com/questions/6117011/how-do-i-get-all-possible-combinations-of-an-enum-flags
    /// <summary>
    /// Get The list of all possible combinations of enums for an enum type with the [Flags] attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static List<T> IterateFlagCombinations<T>()
        where T : struct, Enum
    {
        // Unneeded if you add T : struct, Enum
        if (typeof(T).BaseType != typeof(Enum)) throw new ArgumentException("T must be an Enum type");

        // The return type of Enum.GetValues is Array but it is effectively int[] per docs
        // This bit converts to int[]
        var values = Enum.GetValues(typeof(T)).Cast<int>().ToList();

        if (!typeof(T).IsFlags())
        {
            // We don't have flags so just return the result of GetValues
            return values.Cast<T>().ToList();
        }

        var valuesInverted = values.Select(v => ~v).ToArray();
        int max = 0;
        for (int i = 0; i < values.Count; i++)
        {
            max |= values[i];
        }

        var result = new List<T>();
        for (int i = 0; i <= max; i++)
        {
            int unaccountedBits = i;
            for (int j = 0; j < valuesInverted.Length; j++)
            {
                // This step removes each flag that is set in one of the Enums thus ensuring that
                // an Enum with missing bits won't be passed an int that has those bits set
                unaccountedBits &= valuesInverted[j];
                if (unaccountedBits == 0)
                {
                    result.Add((T)(object)i);
                    break;
                }
            }
        }

        //Check for zero
        try
        {
            if (string.IsNullOrEmpty(Enum.GetName(typeof(T), (T)(object)0)))
            {
                result.Remove((T)(object)0);
            }
        }
        catch
        {
            result.Remove((T)(object)0);
        }

        return result;
    }

    public static bool TryGetEnumValue<T>(this string s, out T v) where T : struct, Enum
    {
        v = default(T);
        if (string.IsNullOrEmpty(s))
            return false;
        var models = GetEnumViewModels<T>();
        var model = models.SingleOrDefault(x => x.Matches(s));
        if (model == null)
            return false;
        v = model.Value;
        return true;
    }
    public static T ToEnumValue<T>(this string s) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
        {
            if (Enum.TryParse(s, out T result))
                return result;
        }
        else
        {
            if (s.TryGetEnumValue(out T result))
                return result;
        }
        throw new ArgumentException($"{s} does not match the short name or alias of any {nameof(T)}");
    }

    public static List<T> GetEnumValues<T>()
        where T : struct, Enum
        => Enum.GetValues<T>().ToList();
    enum EnumAttributeNameType { ID, Display, Short, Description, Group };

    private static string GetEnumAttributeName<TEnum>(this TEnum enumValue, EnumAttributeNameType nameType) where TEnum : struct, Enum
    {
        if (IsFlags<TEnum>() && !Enum.GetValues<TEnum>().Contains(enumValue))
        {
            var values = enumValue.SplitFlags();
            var names = values.Select(x => x.GetEnumAttributeName(nameType)).ToList();
            var joined = string.Join(", ", names);
            return joined;
        }
        string attributeNameValue = nameType switch
        {
            EnumAttributeNameType.ID => enumValue.ToN().ToString(),
            EnumAttributeNameType.Display => enumValue.GetAttribute<TEnum, DisplayAttribute>()?.Name,
            EnumAttributeNameType.Short => enumValue.GetAttribute<TEnum, DisplayAttribute>()?.ShortName,
            EnumAttributeNameType.Description => enumValue.GetAttribute<TEnum, DisplayAttribute>()?.Description,
            EnumAttributeNameType.Group => enumValue.GetAttribute<TEnum, DisplayAttribute>()?.GroupName,
            _ => throw new ArgumentException($"Invalid attribute name type requested for enum: {typeof(TEnum).Name}.{enumValue}.", nameof(nameType))
        };
        if (attributeNameValue == null)
            return enumValue.ToString();
        return attributeNameValue;
    }
    public static string GetID<T>(this T enumValue) where T : struct, Enum
        => enumValue.GetEnumAttributeName(EnumAttributeNameType.Display);
    public static string GetDisplayName<T>(this T enumValue) where T : struct, Enum
        => enumValue.GetEnumAttributeName(EnumAttributeNameType.Display);
    public static string GetShortName<T>(this T enumValue) where T : struct, Enum
        => enumValue.GetEnumAttributeName(EnumAttributeNameType.Short);
    public static string GetEnumGroupNamme<T>(this T enumValue) where T : struct, Enum
        => enumValue.GetEnumAttributeName(EnumAttributeNameType.Group);
    public static string GetEnumDescription<T>(this T enumValue) where T : struct, Enum
        => enumValue.GetEnumAttributeName(EnumAttributeNameType.Description);
    public static bool IsFlags(this Type t)
        => t.GetCustomAttribute<FlagsAttribute>() != null;
    public static bool IsFlags<TEnum>()
        where TEnum : struct, Enum
        => GetAttribute<TEnum, FlagsAttribute>() != null;

    public static List<TEnum> SplitFlags<TEnum>(this TEnum enumValue)
        where TEnum : struct, Enum
    {
        if (!IsFlags<TEnum>())
            throw new ArgumentException($"Input enum type must have the Flags attribute.");
        var values = GetEnumValues<TEnum>();
        var flags = GetEnumValues<TEnum>()
            .Where(e => e.ToN() != 0)
            .Where(e => enumValue.HasFlag(e))
            .ToList()
        ;
        return flags;
    }
    public static TAttribute GetAttribute<TEnum, TAttribute>()
        where TEnum : struct, Enum
        where TAttribute : Attribute
    => typeof(TEnum).GetCustomAttribute<TAttribute>();
    public static TAttribute GetAttribute<TEnum, TAttribute>(this TEnum enumValue)
        where TEnum : struct, Enum
        where TAttribute : Attribute
    => enumValue.GetAttributes<TEnum,TAttribute>().SingleOrDefault();
    public static List<TAttribute> GetAttributes<TEnum, TAttribute>(this TEnum enumValue) where TEnum : struct, Enum where TAttribute : Attribute
    {
        string enumValueAsString = enumValue.ToString();

        var type = enumValue.GetType();
        var fieldInfo = type.GetField(enumValueAsString);
        var attributes = fieldInfo.GetCustomAttributes().OfType<TAttribute>().ToList();
        return attributes;
    }
    public static List<string> GetAttributeProperties<TEnum, TAttribute>(this TEnum enumValue, string propertyName) where TEnum : struct, Enum where TAttribute : Attribute
    {
        string enumValueAsString = enumValue.ToString();

        var type = enumValue.GetType();
        var fieldInfo = type.GetField(enumValueAsString);
        var attributes = fieldInfo.GetCustomAttributes().OfType<TAttribute>();

        var collection = new List<string>();
        foreach (var att in attributes)
        {
            string propertyValue = typeof(TAttribute).GetProperty(propertyName).GetValue(att, null)?.ToString();
            collection.Add(propertyValue);
        }
        return collection;
    }

    public static TEnum? GetEnumValueByAttribute<TEnum, TAttribute>(string fieldName) 
        where TEnum : struct, Enum 
        where TAttribute : Attribute
    {
        var fieldInfo = typeof(TEnum).GetField(fieldName);
        var attribute = fieldInfo.GetCustomAttributes().OfType<TAttribute>().SingleOrDefault();
        if (attribute == null)
            return default;
        var enumValue = (TEnum)fieldInfo.GetRawConstantValue();
        return enumValue;
    }

    public static bool TryParse<T>(this string input, out List<T> results, params char[] delimiters) where T : struct, Enum
    {
        results = new List<T>();
        if (string.IsNullOrWhiteSpace(input))
            return true;
        var parts = input.Split(delimiters).Select(x => x.Trim());
        foreach (string part in parts)
        {
            if (Enum.TryParse(part, out T result))
            {
                results.Add(result);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}