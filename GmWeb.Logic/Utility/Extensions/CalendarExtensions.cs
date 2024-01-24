using System;
using System.ComponentModel;
using System.Linq;
using iCalEvent = Ical.Net.CalendarComponents.CalendarEvent;
using iCalProperty = Ical.Net.CalendarProperty;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using GmWeb.Logic.Utility.Extensions.Reflection;
using CalendarPropertyAttribute = GmWeb.Logic.Data.Annotations.CalendarPropertyAttribute;
using System.Reflection;

namespace GmWeb.Logic.Utility.Extensions.Calendar;
public static class CalendarExtensions
{
    public static void SetProperty<TProperty>(this iCalEvent calEvent, string propertyName, TProperty propertyValue)
    {
        propertyName = propertyName.ToUpper();
        if (calEvent.Properties.ContainsKey(propertyName))
        {
            var property = calEvent.Properties[propertyName];
            if (propertyValue == null)
                calEvent.Properties.Remove(property);
            else
                property.SetValue(propertyValue.ToString());
        }
        else if (propertyValue != null)
        {
            var property = new iCalProperty(propertyName, propertyValue.ToString());
            calEvent.AddProperty(property);
        }
    }
    public static void SetProperty(this iCalEvent calEvent, PropertyInfo propertyInfo, object propertyValue)
    {
        var propertyName = propertyInfo.Name.ToUpper();
        if (calEvent.Properties.ContainsKey(propertyName))
        {
            var property = calEvent.Properties[propertyName];
            if (propertyValue == null)
                calEvent.Properties.Remove(property);
            else
                property.SetValue(propertyValue.ToString());
        }
        else if (propertyValue != null)
        {
            var property = new iCalProperty(propertyName, propertyValue.ToString());
            calEvent.AddProperty(property);
        }
    }

    private static TValue ConvertNumericType<TValue>(object value)
    {
        if (typeof(TValue) == typeof(string))
            return (TValue)value;
        var t = typeof(TValue).UnboxNullable();
        var converter = TypeDescriptor.GetConverter(t);
        var rawValue = converter.ConvertFrom(value?.ToString());
        TValue result = (TValue)rawValue;
        return result;
    }
    private static object ConvertNumericType(this Type t, object value)
    {
        if (t == typeof(string))
            return value?.ToString();
        t = t.UnboxNullable();
        var converter = TypeDescriptor.GetConverter(t);
        var converted = converter.ConvertFrom(value?.ToString());
        return converted;
    }
    public static void StoreProperties<TEntity>(this TEntity calendarModel, iCalEvent calEvent)
    {
        var calPropInfos = calendarModel.GetPropertiesByAttribute<CalendarPropertyAttribute>().Select(x => x.Property);
        foreach (var propInfo in calPropInfos)
        {
            var value = propInfo.GetValue(calendarModel);
            calEvent.SetProperty(propInfo, value);
        }
    }
    public static void LoadProperties<TEntity>(this TEntity calendarModel, iCalEvent calEvent)
    {
        var calPropInfos = calendarModel.GetPropertiesByAttribute<CalendarPropertyAttribute>().Select(x => x.Property);
        foreach (var propInfo in calPropInfos)
        {
            if (calEvent.TryGetProperty(propInfo, out object result))
            {
                propInfo.SetValue(calendarModel, result);
            }
        }
    }
    public static bool TryGetProperty<TProperty>(this iCalEvent calEvent, string propertyName, out TProperty? result)
        where TProperty : struct
    {
        if (calEvent.TryGetProperty(propertyName, out TProperty innerResult))
        {
            result = innerResult;
            return true;
        }
        result = null;
        return false;
    }
    public static bool TryGetProperty<TProperty>(this iCalEvent calEvent, string propertyName, out TProperty result)
    {
        propertyName = propertyName.ToUpper();
        result = default(TProperty);
        if (!calEvent.Properties.ContainsKey(propertyName))
            return false;
        try
        {
            var prop = calEvent.Properties[propertyName];
            result = ConvertNumericType<TProperty>(prop.Value);
            return true;
        }
        catch
        {
            // TODO: Handle unexpected exceptions
        }
        return false;
    }
    public static bool TryGetProperty(this iCalEvent calEvent, PropertyInfo propertyInfo, out object result)
    {
        var propertyName = propertyInfo.Name.ToUpper();
        result = default;
        if (!calEvent.Properties.ContainsKey(propertyName))
            return false;
        try
        {
            var prop = calEvent.Properties[propertyName];
            result = ConvertNumericType(propertyInfo.PropertyType, prop.Value);
            return true;
        }
        catch
        {
            // TODO: Handle unexpected exceptions
        }
        return false;
    }
}