using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Radilovsoft.Rest.Infrastructure.Helpers;

namespace Radilovsoft.Rest.Infrastructure.Test.Helpers
{
    public class ExpressionHelperTest
    {
        private ExpressionHelper _expressionHelper;

        [SetUp]
        public void SetUp()
        {
            _expressionHelper = new ExpressionHelper();
        }

        [Test]
        public void TestExpression()
        {
            var testDto = new TestDto {Id = Guid.NewGuid()};
            var fieldName = nameof(TestDto.Id);
            var type = typeof(TestDto);
            var arg = Expression.Parameter(type, "x");
            var parsFieldToExpression =_expressionHelper.ParsFieldToExpression(fieldName, type, arg) as MemberExpression;
            Assert.NotNull(parsFieldToExpression);
            var expression = Expression.Lambda<Func<TestDto, Guid>>(parsFieldToExpression, arg);
            var value = expression.Compile().Invoke(testDto);
            Assert.IsTrue(value == testDto.Id);
        }
        
        [Test]
        public void TestExpression1()
        {
            var testDto = new TestDto {SubTest = new TestDto {Id = Guid.NewGuid()}};
            var fieldName = $"{nameof(TestDto.SubTest)}.{nameof(TestDto.Id)}";
            var type = typeof(TestDto);
            var arg = Expression.Parameter(type, "x");
            var parsFieldToExpression =_expressionHelper.ParsFieldToExpression(fieldName, type, arg) as MemberExpression;
            Assert.NotNull(parsFieldToExpression);
            var expression = Expression.Lambda<Func<TestDto, Guid>>(parsFieldToExpression, arg);
            var value = expression.Compile().Invoke(testDto);
            Assert.IsTrue(value == testDto.SubTest.Id);
        }
    }
}