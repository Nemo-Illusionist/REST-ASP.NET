using System.Linq;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Service
{
    public interface IOrderService
    {
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, Order[] orders);
    }
}