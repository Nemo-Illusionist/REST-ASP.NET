using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Morcatko.AspNetCore.JsonMergePatch;
using REST.DataCore.Contract;
using REST.DataCore.Contract.Entity;
using REST.DataCore.Contract.Provider;
using REST.Infrastructure.Contract;
using REST.Infrastructure.Contract.Dto;
using REST.Infrastructure.Dto;
using REST.Infrastructure.Extension;

namespace REST.Infrastructure.Service
{
    public class BaseRoService<TDb, TKey, TDto, TFullDto> : IBaseRoService<TDb, TKey, TDto, TFullDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
    {
        protected IDataProvider DataProvider { get; }
        protected IAsyncHelpers AsyncHelpers { get; }
        protected IOrderHelper OrderHelper { get; }
        protected IMapper Mapper { get; }

        public BaseRoService([NotNull] IDataProvider dataProvider,
            [NotNull] IAsyncHelpers asyncHelpers,
            [NotNull] IOrderHelper orderHelper,
            [NotNull] IMapper mapper)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            AsyncHelpers = asyncHelpers ?? throw new ArgumentNullException(nameof(asyncHelpers));
            OrderHelper = orderHelper ?? throw new ArgumentNullException(nameof(orderHelper));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<TFullDto> GetById(TKey id)
        {
            var queryable = DataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id))
                .ProjectTo<TFullDto>(Mapper);
            return AsyncHelpers.SingleOrDefaultAsync(queryable);
        }

        public async Task<PagedResult<TDto>> GetByFilter([NotNull] IPageFilter pageFilter,
            Expression<Func<TDto, bool>> filter = null,
            IOrder[] orders = null)
        {
            if (pageFilter == null) throw new ArgumentNullException(nameof(pageFilter));

            var queryable = GetQueryable(pageFilter, filter, orders, false);
            var queryableForCount = GetQueryable(pageFilter, filter, orders, true);


            return new PagedResult<TDto>
            {
                Data = await AsyncHelpers.ToArrayAsync(queryable).ConfigureAwait(false),
                Meta = new Meta
                {
                    Page = pageFilter.Page,
                    PageSize = pageFilter.PageSize,
                    Count = await AsyncHelpers.LongCountAsync(queryableForCount).ConfigureAwait(false)
                }
            };
        }

        private IQueryable<T> GetQueryable<T>(IPageFilter pageFilter, Expression<Func<T, bool>> filter,
            IOrder[] orders, bool isCount)
        {
            var queryable = DataProvider.GetQueryable<TDb>().ProjectTo<T>(Mapper);

            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            if (!isCount)
            {
                if (orders != null && orders.Any())
                {
                    queryable = OrderHelper.ApplyOrderBy(queryable, orders);
                }

                if (pageFilter != null)
                {
                    queryable = queryable.FilterPage(pageFilter);
                }
            }

            return queryable;
        }
    }

    public class BaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        : BaseRoService<TDb, TKey, TDto, TFullDto>, IBaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
        where TRequest : class
    {
        public BaseCrudService([NotNull] IDataProvider dataProvider,
            [NotNull] IAsyncHelpers asyncHelpers,
            [NotNull] IOrderHelper orderHelper,
            [NotNull] IMapper mapper)
            : base(dataProvider, asyncHelpers, orderHelper, mapper)
        {
        }

        public async Task<TKey> Post(TRequest request)
        {
            var db = Mapper.Map<TDb>(request);
            await DataProvider.InsertAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public async Task<TKey> Put(TKey id, TRequest request)
        {
            var db = await GetDbById(id).ConfigureAwait(false);
            db = Mapper.Map(request, db);
            await DataProvider.UpdateAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public async Task<TKey> Patch(TKey id, [NotNull] JsonMergePatchDocument<TRequest> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var db = await GetDbById(id).ConfigureAwait(false);
            db = request.ApplyTo(db);
            await DataProvider.UpdateAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public Task Delete(TKey id)
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
            return AsyncHelpers.SingleAsync(DataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id)));
        }
    }
}