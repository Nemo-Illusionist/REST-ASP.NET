using System.Collections.Generic;
using JetBrains.Annotations;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Contract.Dto
{
    [PublicAPI]
    public interface IOrder
    {
        public SortDirection DirectionValue { get; }
        public IEnumerable<string> SplitField();
    }
}