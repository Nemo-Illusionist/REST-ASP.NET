using System;
using System.Globalization;
using System.Linq.Expressions;
using JetBrains.Annotations;
using REST.Infrastructure.Contract;
using REST.Infrastructure.Dto;
using REST.Infrastructure.Extension;

namespace REST.Infrastructure.Service
{
    public class FilterHelper : IFilterHelper
    {
        private readonly IExpressionHelper _expressionHelper;

        public FilterHelper([NotNull] IExpressionHelper expressionHelper)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter));
            }
        }

        protected virtual OperatorType ParsValue([NotNull] string operatorValue)
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

            if (propertyType == typeof(Guid))
            {
                value = Guid.Parse(value.ToString());
            }
            else if (propertyType == typeof(Guid?))
            {
                value = value != null ? Guid.Parse(value.ToString()) : (object) null;
            }

            return Expression.Constant(value, propertyType);
        }

        protected virtual Expression GetRestrictionExpression(string operatorValue, Expression propertyExpression,
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