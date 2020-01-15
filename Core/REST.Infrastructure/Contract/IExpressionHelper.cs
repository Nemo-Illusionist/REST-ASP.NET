using System;
using System.Linq.Expressions;

namespace REST.Infrastructure.Contract
{
    public interface IExpressionHelper
    {
        Expression ParsFieldToExpression(string field, Type type, ParameterExpression param);
    }
}