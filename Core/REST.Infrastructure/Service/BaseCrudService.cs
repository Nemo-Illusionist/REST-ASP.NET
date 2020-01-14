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
    public class BaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        : IBaseCrudService<TDb, TKey, TDto, TFullDto, TRequest>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
        where TRequest : class
    {
        private readonly IDataProvider _dataProvider;
        private readonly IAsyncHelpers _asyncHelpers;
        private readonly IOrderHelper _orderHelper;
        private readonly IMapper _mapper;

        public BaseCrudService([NotNull] IDataProvider dataProvider,
            [NotNull] IAsyncHelpers asyncHelpers,
            [NotNull] IOrderHelper orderHelper,
            [NotNull] IMapper mapper)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _asyncHelpers = asyncHelpers ?? throw new ArgumentNullException(nameof(asyncHelpers));
            _orderHelper = orderHelper ?? throw new ArgumentNullException(nameof(orderHelper));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<TFullDto> GetById(TKey id)
        {
            var queryable = _dataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id))
                .ProjectTo<TFullDto>(_mapper);
            return _asyncHelpers.SingleOrDefaultAsync(queryable);
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
                Data = await _asyncHelpers.ToArrayAsync(queryable).ConfigureAwait(false),
                Meta = new Meta
                {
                    Page = pageFilter.Page,
                    PageSize = pageFilter.PageSize,
                    Count = await _asyncHelpers.LongCountAsync(queryableForCount).ConfigureAwait(false)
                }
            };
        }

        public async Task<TKey> Post(TRequest request)
        {
            var db = _mapper.Map<TDb>(request);
            await _dataProvider.InsertAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public async Task<TKey> Put(TKey id, TRequest request)
        {
            var db = await GetDbById(id).ConfigureAwait(false);
            db = _mapper.Map(request, db);
            await _dataProvider.UpdateAsync(db).ConfigureAwait(false);
            return db.Id;
        }

        public async Task<TKey> Patch(TKey id, [NotNull] JsonMergePatchDocument<TRequest> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var db = await GetDbById(id).ConfigureAwait(false);
            db = request.ApplyTo(db);
            await _dataProvider.UpdateAsync(db).ConfigureAwait(false);
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
                return _dataProvider.DeleteByIdAsync<TDb, TKey>(id);
            }
        }

        private Task<TDb> GetDbById(TKey id)
        {
            return _asyncHelpers.SingleAsync(_dataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id)));
        }


        private IQueryable<T> GetQueryable<T>(IPageFilter pageFilter, Expression<Func<T, bool>> filter,
            IOrder[] orders, bool isCount)
        {
            var queryable = _dataProvider.GetQueryable<TDb>().ProjectTo<T>(_mapper);

            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            if (!isCount)
            {
                if (orders != null && orders.Any())
                {
                    queryable = _orderHelper.ApplyOrderBy(queryable, orders);
                }

                if (pageFilter != null)
                {
                    queryable = queryable.FilterPage(pageFilter);
                }
            }

            return queryable;
        }
    }
}