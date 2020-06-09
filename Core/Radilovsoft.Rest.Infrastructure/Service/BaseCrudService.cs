using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Data.Core.Contract.Provider;
using Radilovsoft.Rest.Infrastructure.Contract;

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

        public virtual async Task<TKey> Post(TRequest request)
        {
            var db = Mapper.Map<TDb>(request);
            await DataProvider.InsertAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public virtual async Task<TKey> Put(TKey id, TRequest request)
        {
            var db = await GetDbById(id).ConfigureAwait(false);
            db = Mapper.Map(request, db);
            await DataProvider.UpdateAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public virtual Task Delete(TKey id)
        {
            if (typeof(TDb).IsAssignableFrom(typeof(IDeletable)))
            {
                var methodInfo = typeof(IDeletable).GetMethod(nameof(IDataProvider.SetDeleteAsync));
                //todo: Исправить!
                if (methodInfo == null) throw new InvalidOperationException();

                //todo: Проверить!
                var setDeleteDelegate = (Func<TKey, Task>) methodInfo
                    .MakeGenericMethod(typeof(TDb), typeof(TKey))
                    .CreateDelegate(typeof(Func<TKey, Task>));
                return setDeleteDelegate(id);
            }
            else
            {
                return DataProvider.DeleteByIdAsync<TDb, TKey>(id);
            }
        }

        private Task<TDb> GetDbById(TKey id)
        {
            return AsyncHelpers.SingleAsync(RoDataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id)));
        }
    }
}