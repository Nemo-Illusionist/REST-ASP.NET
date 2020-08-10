using System;
using System.Linq;
using NUnit.Framework;
using Radilovsoft.Rest.Infrastructure.Dto;
using Radilovsoft.Rest.Infrastructure.Helpers;
using Radilovsoft.Rest.Infrastructure.Test.Helpers;

namespace Radilovsoft.Rest.Infrastructure.Test
{
    public class FilterHelperTest : BaseHelperTest
    {
        private FilterHelper _filterHelper;
        private IQueryable<TestDto> _queryable;

        [SetUp]
        public void SetUp()
        {
            var gen = new Random();
            _filterHelper = new FilterHelper(new ExpressionHelper());
            _queryable = Enumerable.Range(0, 100)
                .Select(x => new TestDto
                {
                    Id = Guid.NewGuid(),
                    Order = x + 1,
                    Name = $"name{x}",
                    CreatedUtc = RandomDay(gen)
                })
                .ToArray()
                .AsQueryable();
        }

        [TestCase("=")]
        [TestCase("==")]
        [TestCase("equal")]
        public void EqualTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 5,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order == 5).ToArray();
            Equally(q1, q2);
        }

        [TestCase("<>")]
        [TestCase("!=")]
        [TestCase("notequal")]
        public void NotEqualTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 5,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order != 5).ToArray();
            Equally(q1, q2);
        }

        [TestCase("<")]
        [TestCase("less")]
        public void LessTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 50,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order < 50).ToArray();
            Equally(q1, q2);
        }

        [TestCase("<=")]
        [TestCase("lessorequal")]
        public void LessOrEqualTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 50,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order <= 50).ToArray();
            Equally(q1, q2);
        }

        [TestCase(">")]
        [TestCase("greater")]
        public void GreaterTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 50,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order > 50).ToArray();
            Equally(q1, q2);
        }

        [TestCase(">=")]
        [TestCase("greaterorequal")]
        public void GreaterOrEqualTest(string @operator)
        {
            var filter = new Filter
            {
                Field = nameof(TestDto.Order),
                Value = 50,
                Operator = @operator
            };
            var expression = _filterHelper.ToExpression<TestDto>(filter);

            var q1 = _queryable.Where(expression).ToArray();
            var q2 = _queryable.Where(x => x.Order >= 50).ToArray();
            Equally(q1, q2);
        }
    }
}