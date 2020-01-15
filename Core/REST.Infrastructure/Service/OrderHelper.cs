using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using REST.Infrastructure.Contract;
using REST.Infrastructure.Contract.Dto;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Service
{
    public class OrderHelper : IOrderHelper
    {
        private readonly IExpressionHelper _expressionHelper;
        private static readonly IReadOnlyDictionary<string, MethodInfo> Methods = GetMethods();

        private static Dictionary<string, MethodInfo> GetMethods()
        {
            return new[]
            {
                nameof(Queryable.OrderBy),
                nameof(Queryable.OrderByDescending),
                nameof(Queryable.ThenBy),
                nameof(Queryable.ThenByDescending)
            }.ToDictionary(methodName => methodName, GetMethod);
        }

        private static MethodInfo GetMethod(string methodName)
        {
            return typeof(Queryable).GetMethods()
                .Single(method => method.Name == methodName
                                  && method.IsGenericMethodDefinition
                                  && method.GetGenericArguments().Length == 2
                                  && method.GetParameters().Length == 2);
        }


        public OrderHelper([NotNull] IExpressionHelper expressionHelper)
        {
            _expressionHelper =
                expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
        }

        public IQueryable<T> ApplyOrderBy<T>([NotNull] IQueryable<T> queryable, IOrder order)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            if (order == null) return queryable;

            var type = typeof(T);
            return ApplyFirstOrder(queryable, type, order);
        }

        public IQueryable<T> ApplyOrderBy<T>([NotNull] IQueryable<T> queryable, IOrder[] orders)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));

            if (orders == null || !orders.Any()) return queryable;

            var orderFirst = orders.First();
            var type = typeof(T);
            var orderedQueryable = ApplyFirstOrder(queryable, type, orderFirst);

            foreach (var order in orders.Skip(1))
            {
                var methodName = order.DirectionValue == SortDirection.Asc
                    ? nameof(Queryable.ThenBy)
                    : nameof(Queryable.ThenByDescending);
                orderedQueryable = ApplyOrder(orderedQueryable, type, methodName, order.Field);
            }

            return orderedQueryable;
        }

        private IOrderedQueryable<T> ApplyFirstOrder<T>(IQueryable<T> queryable, Type typeOut, IOrder order)
        {
            var methodName = order.DirectionValue == SortDirection.Asc
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            return ApplyOrder(queryable, typeOut, methodName, order.Field);
        }

        private IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, Type type, string methodName,
            string field)
        {
            var typeIn = type;
            var arg = Expression.Parameter(typeIn, "x");
            var expr = _expressionHelper.ParsFieldToExpression(field, typeIn, arg);

            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), typeIn), expr, arg);

            var result = Methods[methodName].MakeGenericMethod(typeof(T), typeIn)
                .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<T>) result;
        }
    }
}