using System.Linq;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;

namespace Radilovsoft.Rest.Infrastructure.Contract
{
    public interface IOrderHelper
    {
        IOrderedQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder order);
        IOrderedQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder[] orders);
    }
}