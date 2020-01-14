using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using REST.Infrastructure.Contract;
using REST.Infrastructure.Dto;
using REST.Infrastructure.Extension;

namespace REST.Infrastructure.Service
{
    public class OrderHelper : IOrderHelper
    {
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


        public IQueryable<T> ApplyOrderBy<T>(IQueryable<T> queryable, Order[] orders)
        {
            if (orders == null || !orders.Any()) return queryable;

            var type = typeof(T);
            var orderFirst = orders.First();
            var sortDirections = orderFirst.Direction.GetOrDefault();
            var methodName = sortDirections == SortDirection.Asc
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            var orderedQueryable = ApplyOrder(queryable, type, methodName, orderFirst.Fields);

            foreach (var order in orders.Skip(1))
            {
                methodName = sortDirections == SortDirection.Asc
                    ? nameof(Queryable.ThenBy)
                    : nameof(Queryable.ThenByDescending);
                orderedQueryable = ApplyOrder(orderedQueryable, type, methodName, order.Fields);
            }

            return orderedQueryable;
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, Type type, string methodName,
            IEnumerable<string> props)
        {
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop.ToUpperCaseFirstChar());
                if (pi == null) continue;

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), type), expr, arg);

            var result = Methods[methodName].MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<T>) result;
        }
    }
}