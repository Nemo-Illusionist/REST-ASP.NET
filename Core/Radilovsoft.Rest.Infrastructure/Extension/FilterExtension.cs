using System;
using Radilovsoft.Rest.Infrastructure.Dto;

namespace Radilovsoft.Rest.Infrastructure.Extension
{
    public static class FilterExtension
    {
        public static bool IsGroup(this Filter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return filter.OperatorEquals(GroupOperatorType.And.ToString())
                   || filter.OperatorEquals(GroupOperatorType.Or.ToString());
        }

        public static bool OperatorEquals(this Filter filter, string value)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return filter.Operator.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}