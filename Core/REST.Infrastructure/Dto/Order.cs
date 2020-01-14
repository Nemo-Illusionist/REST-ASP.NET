using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using REST.Infrastructure.Contract.Dto;
using REST.Infrastructure.Extension;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public class Order : IOrder
    {
        public string Field { get; set; }
        public SortDirection? Direction { get; set; }

        public SortDirection DirectionValue => Direction.GetOrAsc();

        public IEnumerable<string> SplitField()
        {
            return Field.Split('.').Select(x => x.ToUpperCaseFirstChar());
        }
    }
}