using JetBrains.Annotations;
using Radilovsoft.Rest.Infrastructure.Contract.Dto;
using Radilovsoft.Rest.Infrastructure.Extension;

namespace Radilovsoft.Rest.Infrastructure.Dto
{
    [PublicAPI]
    public class Order : IOrder
    {
        public string Field { get; set; }
        public SortDirection? Direction { get; set; }

        public SortDirection DirectionValue => Direction.GetOrAsc();
    }
}