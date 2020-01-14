using System.Linq;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Contract
{
    public interface IOrderService
    {
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, Order[] orders);
    }
}