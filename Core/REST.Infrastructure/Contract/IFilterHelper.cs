using System;
using System.Linq.Expressions;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Contract
{
    public interface IFilterHelper
    {
        Expression<Func<T, bool>> ToExpression<T>(Filter filter);
    }
}