using Radilovsoft.Rest.Infrastructure.Contract.Dto;

namespace Radilovsoft.Rest.Infrastructure.Dto
{
    public class Order : IOrder
    {
        public string Field { get; set; }

        public SortDirection? Direction { get; set; }
    }
}