using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Radilovsoft.Rest.Core.Exception;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Data.Core.Contract.Provider;
using Radilovsoft.Rest.Infrastructure.Contract;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;
using Radilovsoft.Rest.Infrastructure.Dto;
using Radilovsoft.Rest.Infrastructure.Extension;

namespace Radilovsoft.Rest.Infrastructure.Service
{
    public class BaseRoService<TDb, TKey, TDto, TFullDto> : IBaseRoService<TDb, TKey, TDto, TFullDto>
        where TDb : class, IEntity<TKey>
        where TKey : IComparable
        where TDto : class
        where TFullDto : class
    {
        private readonly IRoDataProvider _dataProvider;
        protected IAsyncHelpers AsyncHelpers { get; }
        private readonly IOrderHelper _orderHelper;
        protected IMapper Mapper { get; }

        public BaseRoService([NotNull] IRoDataProvider dataProvider,
            [NotNull] IAsyncHelpers asyncHelpers,
            [NotNull] IOrderHelper orderHelper,
            [NotNull] IMapper mapper)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _orderHelper = orderHelper ?? throw new ArgumentNullException(nameof(orderHelper));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            AsyncHelpers = asyncHelpers ?? throw new ArgumentNullException(nameof(asyncHelpers));
        }

        public async Task<TFullDto> GetById(TKey id)
        {
            var queryable = _dataProvider.GetQueryable<TDb>().Where(x => x.Id.Equals(id))
                .ProjectTo<TFullDto>(Mapper);
            var result = await AsyncHelpers.SingleOrDefaultAsync(queryable).ConfigureAwait(false);

            if (result == null) throw new ItemNotFoundException();
            return result;
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
            var queryable = _dataProvider.GetQueryable<TDb>().ProjectTo<T>(Mapper);

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