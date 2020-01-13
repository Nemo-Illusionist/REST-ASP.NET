using System.Collections.Generic;
using JetBrains.Annotations;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public class Order
    {
        public IEnumerable<string> Fields { get; set; }

        public SortDirection? Direction { get; set; }
    }
}