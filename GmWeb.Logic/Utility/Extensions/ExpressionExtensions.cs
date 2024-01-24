using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GmWeb.Logic.Utility.Extensions.Expressions;

public static class ExpressionExtensions
{
    public static string GetPropertyName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
        => propertySelector.GetProperty().Name;
    public static PropertyInfo GetProperty<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
    {
        var type = typeof(TModel);

        var member = propertySelector.Body as MemberExpression;
        if (member == null)
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                propertySelector.ToString()));

        var propInfo = member.Member as PropertyInfo;
        if (propInfo == null)
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                propertySelector.ToString()));

        if (type != propInfo.ReflectedType &&
            !type.IsSubclassOf(propInfo.ReflectedType))
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a property that is not from type {1}.",
                propertySelector.ToString(),
                type));

        return propInfo;
    }

    public static Expression<Func<TIn, TOut>> CreatePropertySelector<TIn, TOut>(this PropertyInfo property)
    {
        var param = Expression.Parameter(typeof(TIn));
        var body = Expression.PropertyOrField(param, property.Name);
        return Expression.Lambda<Func<TIn, TOut>>(body, param);
    }
}
