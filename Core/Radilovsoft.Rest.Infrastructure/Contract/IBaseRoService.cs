using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
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
        Task<TFullDto> GetByIdAsync(TKey id, CancellationToken token = default);

        Task<PagedResult<TDto>> GetByFilterAsync(
            IPageFilter pageFilter,
            Expression<Func<TDto, bool>> filter = null,
            IOrder[] orders = null,
            CancellationToken token = default);
    }
}