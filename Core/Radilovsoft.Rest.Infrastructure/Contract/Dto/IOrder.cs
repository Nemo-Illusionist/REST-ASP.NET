using Radilovsoft.Rest.Infrastructure.Dto;

namespace Radilovsoft.Rest.Infrastructure.Contract.Dto
{
    public interface IOrder
    {
        public string Field { get; }

        public SortDirection DirectionValue { get; }
    }
}