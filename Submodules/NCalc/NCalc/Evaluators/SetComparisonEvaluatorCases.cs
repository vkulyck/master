
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc.Domain;

namespace NCalc.Evaluators
{
    public static partial class SetComparisonEvaluator
    {
        public static bool? Evaluate(BinaryExpressionType type, object left, object right)
        {
            switch(left)
            {
                  
                case ISet<char> charSet: {
                    var leftSet = left as ISet<char>;
                    var rightSet = right as ISet<char>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<char?> N_charSet: {
                    var leftSet = left as ISet<char?>;
                    var rightSet = right as ISet<char?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<byte> byteSet: {
                    var leftSet = left as ISet<byte>;
                    var rightSet = right as ISet<byte>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<byte?> N_byteSet: {
                    var leftSet = left as ISet<byte?>;
                    var rightSet = right as ISet<byte?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<sbyte> sbyteSet: {
                    var leftSet = left as ISet<sbyte>;
                    var rightSet = right as ISet<sbyte>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<sbyte?> N_sbyteSet: {
                    var leftSet = left as ISet<sbyte?>;
                    var rightSet = right as ISet<sbyte?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<ushort> ushortSet: {
                    var leftSet = left as ISet<ushort>;
                    var rightSet = right as ISet<ushort>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<ushort?> N_ushortSet: {
                    var leftSet = left as ISet<ushort?>;
                    var rightSet = right as ISet<ushort?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<short> shortSet: {
                    var leftSet = left as ISet<short>;
                    var rightSet = right as ISet<short>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<short?> N_shortSet: {
                    var leftSet = left as ISet<short?>;
                    var rightSet = right as ISet<short?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<uint> uintSet: {
                    var leftSet = left as ISet<uint>;
                    var rightSet = right as ISet<uint>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<uint?> N_uintSet: {
                    var leftSet = left as ISet<uint?>;
                    var rightSet = right as ISet<uint?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<int> intSet: {
                    var leftSet = left as ISet<int>;
                    var rightSet = right as ISet<int>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<int?> N_intSet: {
                    var leftSet = left as ISet<int?>;
                    var rightSet = right as ISet<int?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<ulong> ulongSet: {
                    var leftSet = left as ISet<ulong>;
                    var rightSet = right as ISet<ulong>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<ulong?> N_ulongSet: {
                    var leftSet = left as ISet<ulong?>;
                    var rightSet = right as ISet<ulong?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<long> longSet: {
                    var leftSet = left as ISet<long>;
                    var rightSet = right as ISet<long>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<long?> N_longSet: {
                    var leftSet = left as ISet<long?>;
                    var rightSet = right as ISet<long?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<float> floatSet: {
                    var leftSet = left as ISet<float>;
                    var rightSet = right as ISet<float>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<float?> N_floatSet: {
                    var leftSet = left as ISet<float?>;
                    var rightSet = right as ISet<float?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<double> doubleSet: {
                    var leftSet = left as ISet<double>;
                    var rightSet = right as ISet<double>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<double?> N_doubleSet: {
                    var leftSet = left as ISet<double?>;
                    var rightSet = right as ISet<double?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<decimal> decimalSet: {
                    var leftSet = left as ISet<decimal>;
                    var rightSet = right as ISet<decimal>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<decimal?> N_decimalSet: {
                    var leftSet = left as ISet<decimal?>;
                    var rightSet = right as ISet<decimal?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<DateTime> DateTimeSet: {
                    var leftSet = left as ISet<DateTime>;
                    var rightSet = right as ISet<DateTime>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<DateTime?> N_DateTimeSet: {
                    var leftSet = left as ISet<DateTime?>;
                    var rightSet = right as ISet<DateTime?>;
                    return Evaluate(type, leftSet, rightSet);
                }
  
                case ISet<string> stringSet: {
                    var leftSet = left as ISet<string>;
                    var rightSet = right as ISet<string>;
                    return Evaluate(type, leftSet, rightSet);
                }
            }
            return null;
        }              
    }

}
