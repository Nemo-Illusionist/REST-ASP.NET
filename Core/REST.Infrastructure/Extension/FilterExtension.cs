using System;
using JetBrains.Annotations;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Extension
{
    public static class FilterExtension
    {
        public static bool IsGroup([NotNull] this Filter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return filter.OperatorEquals(GroupOperatorType.And.ToString())
                   || filter.OperatorEquals(GroupOperatorType.Or.ToString());
        }

        public static bool OperatorEquals([NotNull] this Filter filter, string value)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return filter.Operator.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}