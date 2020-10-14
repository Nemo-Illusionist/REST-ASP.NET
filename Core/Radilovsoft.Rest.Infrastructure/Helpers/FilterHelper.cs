using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Radilovsoft.Rest.Infrastructure.Contract.Helper;
using Radilovsoft.Rest.Infrastructure.Dto;
using Radilovsoft.Rest.Infrastructure.Extension;

namespace Radilovsoft.Rest.Infrastructure.Helpers
{
    public class FilterHelper : IFilterHelper
    {
        private static readonly MethodInfo InMethod;

        static FilterHelper()
        {
            InMethod = typeof(Enumerable).GetMethods()
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2);
        }

        private readonly IExpressionHelper _expressionHelper;

        public FilterHelper(IExpressionHelper expressionHelper)
        {
            _expressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
        }

        public Expression<Func<T, bool>> ToExpression<T>(Filter filter)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = ToExpressionRec(filter, param, typeof(T));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private Expression ToExpressionRec(Filter filter, ParameterExpression param, Type type)
        {
            if (filter == null) return null;

            if (filter.IsGroup())
            {
                if (filter.Left == null || filter.Right == null)
                {
                    throw new ArgumentException("Filter invalid", nameof(filter));
                }

                var left = ToExpressionRec(filter.Left, param, type);
                var right = ToExpressionRec(filter.Right, param, type);

                return filter.OperatorEquals(GroupOperatorType.And.ToString())
                    ? Expression.And(left, right)
                    : Expression.Or(left, right);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(filter.Field))
                {
                    throw new ArgumentException("Filter invalid", nameof(filter));
                }

                return GetDefaultRestrictionExpression(filter, param, type);
            }
        }

        private Expression GetDefaultRestrictionExpression(Filter filter, ParameterExpression param, Type type)
        {
            var field = filter.Field;
            var prop = _expressionHelper.ParsFieldToExpression(field, type, param);

            var constant = CreateDefaultConstantExpression(filter, type, field);

            var expression = GetRestrictionExpression(filter.Operator, prop, constant);
            if (expression != null) return expression;

            var operatorType = ParsValue(filter.Operator);

            switch (operatorType)
            {
                case OperatorType.Equal:
                    return Expression.Equal(prop, constant);
                case OperatorType.NotEqual:
                    return Expression.NotEqual(prop, constant);
                case OperatorType.Less:
                    return Expression.LessThan(prop, constant);
                case OperatorType.LessOrEqual:
                    return Expression.LessThanOrEqual(prop, constant);
                case OperatorType.Greater:
                    return Expression.GreaterThan(prop, constant);
                case OperatorType.GreaterOrEqual:
                    return Expression.GreaterThanOrEqual(prop, constant);
                case OperatorType.In:
                    return Expression.Call(null, InMethod.MakeGenericMethod(prop.Type), constant, prop);
                    ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter));
            }
        }

        protected virtual OperatorType ParsValue(string operatorValue)
        {
            if (string.IsNullOrEmpty(operatorValue))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(operatorValue));
            }

            switch (operatorValue.ToLower(CultureInfo.CurrentCulture))
            {
                case "=":
                case "==":
                case "equal":
                    return OperatorType.Equal;
                case "<>":
                case "!=":
                case "notequal":
                    return OperatorType.NotEqual;
                case "<":
                case "less":
                    return OperatorType.Less;
                case "<=":
                case "lessorequal":
                    return OperatorType.LessOrEqual;
                case ">":
                case "greater":
                    return OperatorType.Greater;
                case ">=":
                case "greaterorequal":
                    return OperatorType.GreaterOrEqual;
                case "in":
                    return OperatorType.In;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operatorValue));
            }
        }

        private ConstantExpression CreateDefaultConstantExpression(Filter filter, Type type, string field)
        {
            var expression = CreateConstantExpression(filter, type, field);
            if (expression != null) return expression;

            var value = filter.Value;
            var propertyType = type.GetProperty(field)?.PropertyType ?? typeof(object);

            if (value?.GetType().GetInterface(nameof(IEnumerable)) != null)
            {
                propertyType = propertyType.MakeArrayType();
            }

            if (propertyType == typeof(Guid))
            {
                value = Guid.Parse(value?.ToString()!);
            }
            else if (propertyType == typeof(Guid?))
            {
                value = value != null ? Guid.Parse(value.ToString()) : (object) null;
            }

            return Expression.Constant(value, propertyType);
        }

        protected virtual Expression GetRestrictionExpression(
            string operatorValue,
            Expression propertyExpression,
            ConstantExpression constantExpression)
        {
            return null;
        }

        protected virtual ConstantExpression CreateConstantExpression(Filter filter, Type type, string field)
        {
            return null;
        }
    }
}