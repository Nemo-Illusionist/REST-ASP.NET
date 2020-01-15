using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Contract.Dto
{
    public interface IOrder
    {
        public string Field { get; }

        public SortDirection DirectionValue { get; }
    }
}