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
        private static readonly HashSet<BinaryExpressionType> ValidOperators = new HashSet<BinaryExpressionType>
        {
            BinaryExpressionType.Greater, BinaryExpressionType.GreaterOrEqual,
            BinaryExpressionType.LesserOrEqual, BinaryExpressionType.Lesser,
            BinaryExpressionType.Equal, BinaryExpressionType.NotEqual
        };
        public static void Evaluate(BinaryExpressionType type, OperatorArgs args)
        {
            object
                left = args.LeftOperand.Evaluate(),
                right = args.RightOperand.Evaluate()
            ;
            Type 
                leftType = left.GetType(), 
                rightType = right.GetType()
            ;
            if (leftType != rightType)
                return;
            if (!ValidOperators.Contains(type))
                return;
            if (!leftType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISet<>)))
                return;
            var result = Evaluate(type, left, right);
            if (result != null)
                args.Result = result.Value;
        }

        private static bool Evaluate<T>(BinaryExpressionType type, ISet<T> left, ISet<T> right)
        {
            switch (type)
            {
                case BinaryExpressionType.Greater:
                    return left.IsProperSupersetOf(right);
                case BinaryExpressionType.GreaterOrEqual:
                    return left.IsSupersetOf(right);
                case BinaryExpressionType.LesserOrEqual:
                    return left.IsSubsetOf(right);
                case BinaryExpressionType.Lesser:
                    return left.IsProperSubsetOf(right);
                case BinaryExpressionType.Equal:
                    return left.SetEquals(right);
                case BinaryExpressionType.NotEqual:
                    return !left.SetEquals(right);
                default:
                    throw new ArgumentException($"No evaluator defined for binary expressions of type {type}");
            }
        }
        private static bool EvaluateObjectSets(BinaryExpressionType type, ISet<object> left, ISet<object> right)
        {
            switch (type)
            {
                case BinaryExpressionType.Greater:
                    return left.IsProperSupersetOf(right);
                case BinaryExpressionType.GreaterOrEqual:
                    return left.IsSupersetOf(right);
                case BinaryExpressionType.LesserOrEqual:
                    return left.IsSubsetOf(right);
                case BinaryExpressionType.Lesser:
                    return left.IsProperSubsetOf(right);
                case BinaryExpressionType.Equal:
                    return left.SetEquals(right);
                case BinaryExpressionType.NotEqual:
                    return !left.SetEquals(right);
                default:
                    throw new ArgumentException($"No evaluator defined for binary expressions of type {type}");
            }
        }

        public static void ConvertOperandsToObjects(BinaryExpressionType type, OperatorArgs args)
        {
            object
                leftResult = args.LeftOperand.Evaluate(),
                rightResult = args.RightOperand.Evaluate()
            ;
            IEnumerable<object>
                leftItems = (leftResult as IEnumerable)?.Cast<object>(),
                rightItems = (rightResult as IEnumerable)?.Cast<object>()
            ;
            // TODO: These objects should already be HashSet<T>, but the only way to cast with generic parameters
            // would be to switch on the detected type and call a generic version of Evaluate<T>; one switch
            // case per T. It's not clear that there would be any performance improvement to justify the effort.
            ISet<object>
                left = leftItems?.ToHashSet(),
                right = rightItems?.ToHashSet()
            ;
            if (left != null && right != null)
            {
                args.Result = Evaluate(type, left, right);
            }
        }
    }
}
