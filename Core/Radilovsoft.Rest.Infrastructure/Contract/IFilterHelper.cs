using System;
using System.Linq.Expressions;
using Radilovsoft.Rest.Infrastructure.Dto;

namespace Radilovsoft.Rest.Infrastructure.Contract
{
    public interface IFilterHelper
    {
        Expression<Func<T, bool>> ToExpression<T>(Filter filter);
    }
}