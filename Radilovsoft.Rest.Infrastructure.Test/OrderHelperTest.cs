using System;
using System.Linq;
using NUnit.Framework;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;
using Radilovsoft.Rest.Infrastructure.Dto;
using Radilovsoft.Rest.Infrastructure.Helpers;
using Radilovsoft.Rest.Infrastructure.Test.Helpers;

namespace Radilovsoft.Rest.Infrastructure.Test
{
    public class OrderHelperTest : BaseHelperTest
    {
        private OrderHelper _orderHelper;
        private IQueryable<TestDto> _queryable;

        [SetUp]
        public void SetUp()
        {
            var gen = new Random();
            _orderHelper = new OrderHelper(new ExpressionHelper());
            _queryable = Enumerable.Range(0, 100)
                .Select(x => new TestDto {Id = Guid.NewGuid(), Name = $"name{x}", CreatedUtc = RandomDay(gen)})
                .ToArray()
                .AsQueryable();
        }

        [Test]
        public void TestOrderBy()
        {
            var order = new Order {Field = nameof(TestDto.Name), Direction = SortDirection.Desc};

            var queryable1 = _orderHelper.ApplyOrderBy(_queryable, order).ToArray();
            var queryable2 = _queryable.OrderByDescending(x => x.Name).ToArray();

            Equally(queryable2, queryable1);
        }

        [Test]
        public void TestThenBy()
        {
            IOrder[] orders =
            {
                new Order {Field = nameof(TestDto.Name), Direction = SortDirection.Desc},
                new Order {Field = nameof(TestDto.CreatedUtc), Direction = SortDirection.Asc},
            };

            var queryable1 = _orderHelper.ApplyOrderBy(_queryable, orders).ToArray();
            var queryable2 = _queryable.OrderByDescending(x => x.Name).ThenBy(x => x.CreatedUtc).ToArray();

            Equally(queryable2, queryable1);
        }
    }
}