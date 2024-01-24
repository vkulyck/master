using GmWeb.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection;
using GmWeb.Logic.Utility.Extensions.Reflection;
using Newtonsoft.Json;

namespace GmWeb.Logic.Utility.Extensions.Dynamic
{
    public static partial class DynamicValueExtensions
    {
        public static int? GetListReferenceValue(this IDynamicValueType entity) => entity.GetIntValue();
        public static int? GetIntValue(this IDynamicValueType entity)
        {
            if (string.IsNullOrWhiteSpace(entity.RawValue))
                return null;
            if (int.TryParse(entity.RawValue, out int result))
                return result;
            return null;
        }
        public static bool? GetBoolValue(this IDynamicValueType entity)
        {
            if (string.IsNullOrWhiteSpace(entity.RawValue))
                return null;
            if (bool.TryParse(entity.RawValue, out bool result))
                return result;
            if (entity.RawValue == "Yes")
                return true;
            if (entity.RawValue == "No")
                return false;
            return null;
        }

        public static readonly string[] DateTimeFormats = {
            "MM/dd/yyyy",
            "MM/d/yyyy",
            "M/dd/yyyy",
            "M/d/yyyy",
        };
        public static DateTime? GetDateValue(this IDynamicValueType entity, DateTimeStyles style = DateTimeStyles.None)
        {
            if (DateTime.TryParseExact(entity.RawValue, DateTimeFormats, CultureInfo.InvariantCulture, style, out var result))
                return result;
            return null;
        }

        public static void CopyDynamicRowData(this IDynamicValueType entity, System.Data.DataRow row)
        {
            entity.RawValue = row.ToString("Value");
            entity.ParseDataType(row.ToString("DataType"));
        }
        
        public static object ConvertNumericType(this Type t, object value)
        {
            if (!t.IsNumericType())
                throw new Exception($"Attempted to call ConvertNumericType with non-numeric: {t.Name}");
            if (value == null)
                // Return the equivalent of default(T)
                return Activator.CreateInstance(t);
            var code = Type.GetTypeCode(t);
            object converted = Convert.ChangeType(value, code);
            return converted;
        }
        public static bool IsNumericType(this TypeCode tc)
        {
            switch (tc)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsNumericType(this Type t)
        {
            t = t.UnboxNullable();
            if (t.IsEnum)
                return false;
            var code = Type.GetTypeCode(t);
            return code.IsNumericType();
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    var propertyInfo = memberInfo.DeclaringType.GetProperty(memberInfo.Name);
                    return propertyInfo.PropertyType;
                case MemberTypes.Field:
                    var fieldInfo = memberInfo.DeclaringType.GetField(memberInfo.Name);
                    return fieldInfo.FieldType;
                case MemberTypes.Method:
                    var methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
                    return methodInfo.ReturnType;
                default:
                    throw new ArgumentException($"GetMemberType is not configured for MemberTypes.{memberInfo.MemberType}");
            }
        }

        public static Type UnboxNullable(this Type type)
        {
            var ut = Nullable.GetUnderlyingType(type);
            if (ut == null)
                return type;
            return ut;
        }

        public static bool IsMemberNullableNumeric(this MemberInfo memberInfo)
        {
            var memberType = memberInfo.GetMemberType().UnboxNullable();
            bool isNumeric = memberType.IsNumericType();
            return isNumeric;
        }

        private static readonly Dictionary<Guid, SqlDbType> ClrGuidToSqlMap = new()
        {
            [typeof(DateOnly).GUID] = SqlDbType.Date,
            [typeof(TimeOnly).GUID] = SqlDbType.Time,
            [typeof(DateTimeOffset).GUID] = SqlDbType.DateTimeOffset
        };
        public static SqlDbType? ToSqlType(this Type t)
        {
            t = t.UnboxNullable();
            if (ClrGuidToSqlMap.TryGetValue(t.GUID, out SqlDbType result))
                return result;
            return Type.GetTypeCode(t) switch
            {
                TypeCode.Object => SqlDbType.Udt,
                TypeCode.Boolean => SqlDbType.Bit,
                TypeCode.Char => SqlDbType.Char,
                TypeCode.SByte or
                TypeCode.Byte => SqlDbType.TinyInt,
                TypeCode.Int16 or
                TypeCode.UInt16 => SqlDbType.SmallInt,
                TypeCode.Int32 or
                TypeCode.UInt32 => SqlDbType.Int,
                TypeCode.Int64 or
                TypeCode.UInt64 => SqlDbType.BigInt,
                TypeCode.Single => SqlDbType.Float,
                TypeCode.Double => SqlDbType.Real,
                TypeCode.Decimal => SqlDbType.Decimal,
                TypeCode.DateTime => SqlDbType.DateTime2,
                TypeCode.String => SqlDbType.NVarChar,
                TypeCode.Empty => SqlDbType.Variant,
                _ => default
            };
        }
        public static Type ToClrType(this SqlDbType? sqlType)
            => sqlType?.ToClrType();
        public static Type ToClrType(this SqlDbType sqlType)
        {
            return sqlType switch
            {
                SqlDbType.BigInt => typeof(long),
                SqlDbType.Int => typeof(int),
                SqlDbType.SmallInt => typeof(short),
                SqlDbType.TinyInt => typeof(byte),
                SqlDbType.VarBinary or
                SqlDbType.Binary => typeof(byte[]),
                SqlDbType.Bit => typeof(bool),
                SqlDbType.Date => typeof(DateOnly),
                SqlDbType.Time => typeof(TimeOnly),
                SqlDbType.DateTime or
                SqlDbType.DateTime2 or
                SqlDbType.SmallDateTime => typeof(DateTime),
                SqlDbType.DateTimeOffset => typeof(DateTimeOffset),
                SqlDbType.Decimal or
                SqlDbType.Money => typeof(decimal),
                SqlDbType.Float => typeof(float),
                SqlDbType.Real => typeof(double),
                SqlDbType.UniqueIdentifier => typeof(Guid),
                SqlDbType.Char or
                SqlDbType.NChar or
                SqlDbType.VarChar or
                SqlDbType.NVarChar or
                SqlDbType.Text or
                SqlDbType.NText => typeof(string),
                SqlDbType.Xml => typeof(System.Xml.XmlNode),
                SqlDbType.Udt or
                SqlDbType.Variant => typeof(object),
                _ => default
            };
        }
    }
}
