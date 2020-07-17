using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;
using Radilovsoft.Rest.Infrastructure.Dto;
using Radilovsoft.Rest.Infrastructure.Helpers;

namespace Radilovsoft.Rest.Infrastructure.Test.Helpers
{
    public class OrderHelperTest
    {
        private OrderHelper _orderHelper;
        private IQueryable<TestDto> _queryable;

        [SetUp]
        public void SetUp()
        {
            var gen = new Random();
            _orderHelper = new OrderHelper(new ExpressionHelper());
            _queryable = Enumerable.Range(0, 10)
                .Select(x => new TestDto {Id = Guid.NewGuid(), Name = $"name{x}", CreatedUtc = RandomDay(gen)})
                .ToArray()
                .AsQueryable();
        }

        private static DateTime RandomDay(Random gen)
        {
            var start = new DateTime(1995, 1, 1);
            var range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
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

        private static void Equally(IReadOnlyList<TestDto> queryable2, IReadOnlyList<TestDto> queryable1)
        {
            for (int i = 0; i < queryable2.Count; i++)
            {
                if (!Equals(queryable1[i], queryable2[i]))
                {
                    Assert.Fail();
                }
            }
        }
    }
}