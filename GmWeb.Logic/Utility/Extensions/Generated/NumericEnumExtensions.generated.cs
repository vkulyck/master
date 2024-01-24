using System;
using System.Collections.Generic;
using System.Linq;
namespace GmWeb.Logic.Utility.Extensions.Enums;

public static partial class EnumExtensions
{
        public static bool TryGetEnumValue<T>(this Byte number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToByte()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this Byte number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static Byte ToByte<T>(this T e) where T : struct, Enum
    {
        Byte converted = Convert.ToByte(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this SByte number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToSByte()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this SByte number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static SByte ToSByte<T>(this T e) where T : struct, Enum
    {
        SByte converted = Convert.ToSByte(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this Int16 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToShort()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this Int16 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static Int16 ToShort<T>(this T e) where T : struct, Enum
    {
        Int16 converted = Convert.ToInt16(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this UInt16 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToUShort()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this UInt16 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static UInt16 ToUShort<T>(this T e) where T : struct, Enum
    {
        UInt16 converted = Convert.ToUInt16(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this Int32 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToN()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this Int32 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static Int32 ToN<T>(this T e) where T : struct, Enum
    {
        Int32 converted = Convert.ToInt32(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this UInt32 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToUN()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this UInt32 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static UInt32 ToUN<T>(this T e) where T : struct, Enum
    {
        UInt32 converted = Convert.ToUInt32(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this Int64 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToL()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this Int64 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static Int64 ToL<T>(this T e) where T : struct, Enum
    {
        Int64 converted = Convert.ToInt64(e);
        return converted;
    }
    public static bool TryGetEnumValue<T>(this UInt64 number, out T v) where T : struct, Enum
    {
        v = default(T);
        if (!typeof(T).IsFlags())
        {
            var intValues = Enum.GetValues(typeof(T)).OfType<T>().Select(x => x.ToUL()).ToHashSet();
            if (!intValues.Contains(number))
                return false;
        }
        v = (T)(object)number;
        return true;
    }
    public static T ToEnumValue<T>(this UInt64 number) where T : struct, Enum
    {
        if (typeof(T).IsFlags())
            return (T)(object)number;
        if (number.TryGetEnumValue(out T result))
            return result;
        throw new ArgumentException($"{nameof(T)} does not have an assignment to the integer {number}.");
    }

    public static UInt64 ToUL<T>(this T e) where T : struct, Enum
    {
        UInt64 converted = Convert.ToUInt64(e);
        return converted;
    }
}
