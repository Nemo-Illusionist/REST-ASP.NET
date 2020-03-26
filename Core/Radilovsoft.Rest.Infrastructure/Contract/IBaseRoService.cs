using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;
using Radilovsoft.Rest.Infrastructure.Dto;

namespace Radilovsoft.Rest.Infrastructure.Contract
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