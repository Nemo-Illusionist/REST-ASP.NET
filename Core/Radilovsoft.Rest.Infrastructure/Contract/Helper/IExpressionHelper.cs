using System;
using System.Linq.Expressions;

namespace Radilovsoft.Rest.Infrastructure.Contract.Helper
{
    public interface IExpressionHelper
    {
        Expression ParsFieldToExpression(string field, Type type, ParameterExpression param);
    }
}