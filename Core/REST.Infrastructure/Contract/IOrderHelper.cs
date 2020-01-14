using System.Linq;
using REST.Infrastructure.Contract.Dto;

namespace REST.Infrastructure.Contract
{
    public interface IOrderHelper
    {
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder order);
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder[] orders);
    }
}