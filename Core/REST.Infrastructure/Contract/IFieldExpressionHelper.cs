using System;
using System.Linq.Expressions;

namespace REST.Infrastructure.Contract
{
    public interface IFieldExpressionHelper
    {
        Expression ParsFieldToExpression(string field, Type type, ParameterExpression param);
    }
}