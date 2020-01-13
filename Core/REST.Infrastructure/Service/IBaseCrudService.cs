using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Morcatko.AspNetCore.JsonMergePatch;
using REST.DataCore.EntityContract;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Service
{
    public interface IBaseCrudService<TDb, TKey, TDto, TRequest> : IBaseCrudService<TDb, TKey, TDto, TDto, TRequest>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TRequest : class
    {
    }

    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IBaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
        where TRequest : class
    {
        Task<TFullDto> GetById(TKey id);

        Task<PagedResult<TDto>> GetByFilter(PageFilter pageFilter, Expression<Func<TDto, bool>> filter = null,
            Order[] orders = null);

        Task<TKey> Post(TRequest request);
        Task<TKey> Put(TKey id, TRequest request);
        Task<TKey> Patch(TKey id, JsonMergePatchDocument<TRequest> request);
        Task Delete(TKey id);
    }
}