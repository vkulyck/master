


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GmWeb.Logic.Utility.Extensions.Dynamic
{
    public static partial class DynamicValueExtensions
    {
        public static Expression<Func<object,object>> NumericStringConverterExpression(this Type type, object defaultValue = null)
        {
            var code = Type.GetTypeCode(type);
            return NumericStringConverterExpression(code, defaultValue);
        }
        public static Expression<Func<object, object>> NumericStringConverterExpression(this TypeCode code, object defaultValue = null)
        {
            switch(code)
            {
            
			case TypeCode.Boolean: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Boolean.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Char: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Char.Parse(x.ToString())
                ;
            
                        
			case TypeCode.SByte: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.SByte.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Byte: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Byte.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Int16: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Int16.Parse(x.ToString())
                ;
            
                        
			case TypeCode.UInt16: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.UInt16.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Int32: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Int32.Parse(x.ToString())
                ;
            
                        
			case TypeCode.UInt32: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.UInt32.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Int64: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Int64.Parse(x.ToString())
                ;
            
                        
			case TypeCode.UInt64: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.UInt64.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Single: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Single.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Double: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Double.Parse(x.ToString())
                ;
            
                        
			case TypeCode.Decimal: return (Object x) => 
                    x == null ? defaultValue
                    : x.ToString() == "NULL" ? defaultValue
                    : System.Decimal.Parse(x.ToString())
                ;
            
                        }
            //throw new Exception($"No conversion expression could be generated for type code '{code}'.");
            //return null;
            return (object x) => x;
        }

        public static T ConvertValue<T>(string value)
        {
            var result = ConvertValue(typeof(T), value);
            return (T)result;
        }

		public static object ConvertValue(this Type type, string value)
		{
            if(type.IsAssignableFrom(typeof(string)))
                return value;

			else if(type.IsAssignableFrom(typeof(bool))) {
				if(bool.TryParse(value, out bool bool_Value)) {
                    return bool_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(char))) {
				if(char.TryParse(value, out char char_Value)) {
                    return char_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(sbyte))) {
				if(sbyte.TryParse(value, out sbyte sbyte_Value)) {
                    return sbyte_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(byte))) {
				if(byte.TryParse(value, out byte byte_Value)) {
                    return byte_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(short))) {
				if(short.TryParse(value, out short short_Value)) {
                    return short_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(ushort))) {
				if(ushort.TryParse(value, out ushort ushort_Value)) {
                    return ushort_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(int))) {
				if(int.TryParse(value, out int int_Value)) {
                    return int_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(uint))) {
				if(uint.TryParse(value, out uint uint_Value)) {
                    return uint_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(long))) {
				if(long.TryParse(value, out long long_Value)) {
                    return long_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(ulong))) {
				if(ulong.TryParse(value, out ulong ulong_Value)) {
                    return ulong_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(float))) {
				if(float.TryParse(value, out float float_Value)) {
                    return float_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(double))) {
				if(double.TryParse(value, out double double_Value)) {
                    return double_Value;
                }
			}
            
			else if(type.IsAssignableFrom(typeof(decimal))) {
				if(decimal.TryParse(value, out decimal decimal_Value)) {
                    return decimal_Value;
                }
			}
            
            return null;
		}

        public static bool TryParse<T>(this string value, out T result) where T : struct
            => value.TryParse<T>(NumberStyles.None, CultureInfo.InvariantCulture, out result);
        public static bool TryParse<T>(this string value, NumberStyles style, IFormatProvider format, out T result) where T : struct
        {
            switch(Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    if(value == "0")
                    {
                        result = (T)(object)false;
                        return true;
                    }
                    else if(value == "1")
                    {
                        result = (T)(object)true; ;
                        return true;
                    }
                    else if(Boolean.TryParse(value, out Boolean Boolean_result))
                    {
                        result = (Boolean_result as T?).Value;
                        return true;
                    }
                    break;

			    case TypeCode.SByte:
                    if (SByte.TryParse(value, style, format, out SByte SByte_result))
                    {
                        result = (SByte_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Byte:
                    if (Byte.TryParse(value, style, format, out Byte Byte_result))
                    {
                        result = (Byte_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Int16:
                    if (Int16.TryParse(value, style, format, out Int16 Int16_result))
                    {
                        result = (Int16_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.UInt16:
                    if (UInt16.TryParse(value, style, format, out UInt16 UInt16_result))
                    {
                        result = (UInt16_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Int32:
                    if (Int32.TryParse(value, style, format, out Int32 Int32_result))
                    {
                        result = (Int32_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.UInt32:
                    if (UInt32.TryParse(value, style, format, out UInt32 UInt32_result))
                    {
                        result = (UInt32_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Int64:
                    if (Int64.TryParse(value, style, format, out Int64 Int64_result))
                    {
                        result = (Int64_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.UInt64:
                    if (UInt64.TryParse(value, style, format, out UInt64 UInt64_result))
                    {
                        result = (UInt64_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Single:
                    if (Single.TryParse(value, style, format, out Single Single_result))
                    {
                        result = (Single_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Double:
                    if (Double.TryParse(value, style, format, out Double Double_result))
                    {
                        result = (Double_result as T?).Value;
                        return true;
                    }
                    break;

            
			    case TypeCode.Decimal:
                    if (Decimal.TryParse(value, style, format, out Decimal Decimal_result))
                    {
                        result = (Decimal_result as T?).Value;
                        return true;
                    }
                    break;

            
            }
            result = default(T);
            return false;
        }
    }
}

