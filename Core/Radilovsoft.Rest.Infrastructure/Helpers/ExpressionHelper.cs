using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Radilovsoft.Rest.Core.Extension;
using Radilovsoft.Rest.Infrastructure.Contract.Helper;

namespace Radilovsoft.Rest.Infrastructure.Helpers
{
    public class ExpressionHelper : IExpressionHelper
    {
        public Expression ParsFieldToExpression(string field, Type type, ParameterExpression param)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(field));
            }

            var typeIn = type;
            Expression expr = param;
            foreach (var prop in SplitField(field))
            {
                var pi = typeIn.GetProperty(prop);
                if (pi == null) continue;

                expr = Expression.Property(expr, pi);
                typeIn = pi.PropertyType;
            }

            return expr;
        }

        private static IEnumerable<string> SplitField(string field)
        {
            return field.Split('.').Select(x => x.ToUpperCaseFirstChar());
        }
    }
}