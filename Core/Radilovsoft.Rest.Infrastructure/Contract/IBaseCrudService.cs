using System;
using System.Threading;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
    
namespace Radilovsoft.Rest.Infrastructure.Contract
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
        Task<TKey> PostAsync(TRequest request, CancellationToken token = default);
        Task<TKey> PutAsync(TKey id, TRequest request, CancellationToken token = default);
        Task DeleteAsync(TKey id, CancellationToken token = default);
    }
}