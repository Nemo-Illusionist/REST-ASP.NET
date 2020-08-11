using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Data.Core.Contract.Provider;
using Radilovsoft.Rest.Infrastructure.Contract;
using Radilovsoft.Rest.Infrastructure.Contract.Helper;

namespace Radilovsoft.Rest.Infrastructure.Service
{
    public class BaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        : BaseRoService<TDb, TKey, TDto, TFullDto>, IBaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
        where TRequest : class
    {
        protected IDataProvider DataProvider { get; }

        public BaseCrudService([NotNull] IDataProvider dataProvider,
            [NotNull] IAsyncHelpers asyncHelpers,
            [NotNull] IOrderHelper orderHelper,
            [NotNull] IMapper mapper)
            : base(dataProvider, asyncHelpers, orderHelper, mapper)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public virtual async Task<TKey> PostAsync(TRequest request, CancellationToken token = default)
        {
            var db = Mapper.Map<TDb>(request);
            await DataProvider.InsertAsync(db, token).ConfigureAwait(false);
            return db.Id;
        }

        public virtual async Task<TKey> PutAsync(TKey id, TRequest request, CancellationToken token = default)
        {
            var db = await GetDbById(id, token).ConfigureAwait(false);
            db = Mapper.Map(request, db);
            await DataProvider.UpdateAsync(db, token).ConfigureAwait(false);
            return db.Id;
        }

        public virtual Task DeleteAsync(TKey id, CancellationToken token = default)
        {
            if (typeof(TDb).IsAssignableFrom(typeof(IDeletable)))
            {
                var methodInfo = typeof(IDeletable).GetMethod(nameof(IDataProvider.SetDeleteAsync));
                //todo: Исправить!
                if (methodInfo == null) throw new InvalidOperationException();

                //todo: Проверить!
                var setDeleteDelegate = (Func<TKey, CancellationToken, Task>) methodInfo
                    .MakeGenericMethod(typeof(TDb), typeof(TKey))
                    .CreateDelegate(typeof(Func<TKey, CancellationToken, Task>));
                return setDeleteDelegate(id, token);
            }
            else
            {
                return DataProvider.DeleteByIdAsync<TDb, TKey>(id, token);
            }
        }

        private Task<TDb> GetDbById(TKey id, CancellationToken token = default)
        {
            return AsyncHelpers.SingleAsync(RoDataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id)), token);
        }
    }
}