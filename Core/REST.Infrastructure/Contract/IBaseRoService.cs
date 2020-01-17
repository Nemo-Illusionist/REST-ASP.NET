using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using REST.DataCore.Contract.Entity;
using REST.Infrastructure.Contract.Dto;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Contract
{
    public interface IBaseRoService<TDb, TKey, TDto> : IBaseRoService<TDb, TKey, TDto, TDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
    {
    }

    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IBaseRoService<TDb, TKey, TDto, TFullDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
    {
        Task<TFullDto> GetById(TKey id);

        Task<PagedResult<TDto>> GetByFilter(IPageFilter pageFilter, Expression<Func<TDto, bool>> filter = null,
            IOrder[] orders = null);
    }
}