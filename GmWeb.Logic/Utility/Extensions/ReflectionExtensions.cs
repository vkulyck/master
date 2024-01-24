using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Utility.Extensions.Reflection;
public static class ReflectionExtensions
{
    public static PropertyInfo GetPropertyInfo<TSource>(this TSource source, string name)
        => typeof(TSource).GetProperty(name);
    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> selector)
        => selector.GetPropertyInfo();
    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> selector)
    {
        Type type = typeof(TSource);

        MemberExpression member = null;
        if (selector.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            member = unary.Operand as MemberExpression;
        else if (selector.Body is MemberExpression)
            member = selector.Body as MemberExpression;
        else if (selector.Body is MethodCallExpression)
            throw new ArgumentException(
                $"Expression {selector} refers to a method call, not a property.", nameof(selector)
            );
        else if (selector.Body is ConstantExpression)
            throw new ArgumentException(
                $"Expression {selector} refers to a constant, not a property.", nameof(selector)
            );
        else
            throw new ArgumentException(
                $"Expected a property selector, but got an unexpected lambda expression: {selector}", nameof(selector)
            );

        PropertyInfo propInfo = null;
        if (member.Member is FieldInfo)
            throw new ArgumentException(
                $"Expression {selector} refers to a field, not a property.", nameof(selector)
            );
        else if (member.Member is PropertyInfo)
            propInfo = member.Member as PropertyInfo;
        else if (propInfo == null)
            throw new ArgumentException(
                $"Expected a property selector, but got an unexpected lambda expression: {selector}", nameof(selector)
            );

        if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            throw new ArgumentException(
                $"Unable to assign cast selected property {propInfo.Name} as {type.Name}", nameof(selector)
            );

        return propInfo;
    }
    public static object GetPropertyValue<TSource>(this TSource instance, string propertyName)
        => instance.GetPropertyInfo(propertyName).GetValue(instance);
    public static object GetValue<TSource>(this TSource instance, PropertyInfo property)
        => property.GetValue(instance);
    public static IEnumerable<TProperty> GetPropertyValues<TProperty>(this object instance)
    {
        var props = instance.GetTypedProperties<TProperty>();
        var values = props.Select(prop => prop.GetValue(instance)).OfType<TProperty>();
        return values;
    }
    public static IEnumerable<PropertyInfo> GetTypedProperties<TProperty>(this object instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));
        return instance.GetType().GetProperties().Where(x => x.PropertyType == typeof(TProperty)).ToList();
    }

    public static IEnumerable<(PropertyInfo Property, TAttribute Attribute)> GetPropertiesByAttribute<TAttribute>(this object instance)
        where TAttribute : Attribute
    {
        var pairs = instance.GetType()
            .GetProperties()
            .Select(x => (x, x.GetAttribute<TAttribute>()))
            .Where(x => x.Item2 != null)
        ;
        return pairs;
    }
    public static TAttribute GetAttribute<TAttribute>(this PropertyInfo property) where TAttribute : Attribute
    {
        var attributes = property.GetCustomAttributes().OfType<TAttribute>().ToList();
        var attr = attributes.SingleOrDefault();
        return attr;
    }
    public static bool IsMapped(this PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<NotMappedAttribute>(true);
        bool isMapped = attribute == null;
        return isMapped;
    }
    
    public static bool IsNavigation(this PropertyInfo property)
    {
        var fkAttribute = property.GetCustomAttribute<ForeignKeyAttribute>(true);
        var ivAttribute = property.GetCustomAttribute<InversePropertyAttribute>(true);
        bool isNavigatuion = fkAttribute != null || ivAttribute != null;
        return isNavigatuion;
    }

    public static bool IsRelationalProperty(this PropertyInfo property)
    {
        // An inverse property is always relational
        bool isInverseProperty = property.GetCustomAttribute<InversePropertyAttribute>(true) != null;
        if (isInverseProperty)
            return true;

        // Otherwise, check to see if the property is a collection
        var propType = property.PropertyType;

        // Any mapped EFCore collection should be generic
        if (!propType.IsGenericType)
            return false;

        // Construct the matching IEnumerable<T> and confirm assignability
        var typeArgs = propType.GetGenericArguments();
        var matchingEnumerable = typeof(IEnumerable<>).MakeGenericType(typeArgs);

        // The property type is an IEnumerable<T>
        bool isCollection = matchingEnumerable.IsAssignableFrom(propType);
        if (!isCollection)
            // For now, assume that relational properties are always generic collections.
            // TODO: Implement more sophsticated checks for nullables and other
            // (potentially) non-relational property types.
            return false;

        // If the property type is an array of primitives, it might still be a valid SQL column type (e.g. byte[])
        if (!propType.IsArray || !typeArgs[0].IsPrimitive)
            // If one of these checks is false then the property is definitely a relational collection
            return true;

        // For now, assume that arrays of primitives are non-relational.
        // TODO: Implement more sophsticated checks to rule out byte arrays, char arrays, etc.
        return false;
    }

    public static IEnumerable<Type> GetDescendants(this Type root)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => x.FullName.StartsWith("GmWeb"))
            .SelectMany(s => s.GetTypes())
            .Where(p => root.IsAssignableFrom(p))
        ;
        return types;
    }

    public static Type GetTypeFromName(this string typeName)
    {
        switch (typeName)
        {
            case "sbyte": return typeof(sbyte);
            case "short": return typeof(short);
            case "int": return typeof(int);
            case "long": return typeof(long);

            case "byte": return typeof(byte);
            case "ubyte": return typeof(byte);
            case "ushort": return typeof(ushort);
            case "uint": return typeof(uint);
            case "ulong": return typeof(ulong);

            case "float": return typeof(float);
            case "single": return typeof(float);
            case "double": return typeof(double);
            case "decimal": return typeof(decimal);
            case "string": return typeof(string);
        }
        Type result = null;
        result = Type.GetType(typeName);
        if (result != null) return result;
        result = Type.GetType($"System.{typeName}");
        if (result != null) return result;

        return null; // Give up if nothing has been found
    }
}
