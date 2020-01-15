using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using REST.Core.Extension;
using REST.Infrastructure.Contract;

namespace REST.Infrastructure.Service
{
    public class ExpressionHelper : IExpressionHelper
    {
        public Expression ParsFieldToExpression([NotNull] string field, [NotNull] Type type,
            [NotNull] ParameterExpression param)
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