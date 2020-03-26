using System.Linq;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;

namespace Radilovsoft.Rest.Infrastructure.Contract
{
    public interface IOrderHelper
    {
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder order);
        IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, IOrder[] orders);
    }
}