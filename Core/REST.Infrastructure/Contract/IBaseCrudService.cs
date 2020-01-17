using System;
using System.Threading.Tasks;
using Morcatko.AspNetCore.JsonMergePatch;
using REST.DataCore.Contract.Entity;

namespace REST.Infrastructure.Contract
{
    public interface IBaseCrudService<TDb, TKey, TDto, TRequest> : IBaseCrudService<TDb, TKey, TDto, TDto, TRequest>,
        IBaseRoService<TDb, TKey, TDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TRequest : class
    {
    }

    public interface IBaseCrudService<TDb, TKey, TDto, TFullDto, TRequest> : IBaseRoService<TDb, TKey, TDto, TFullDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
        where TRequest : class
    {
        Task<TKey> Post(TRequest request);
        Task<TKey> Put(TKey id, TRequest request);
        Task<TKey> Patch(TKey id, JsonMergePatchDocument<TRequest> request);
        Task Delete(TKey id);
    }
}