using JetBrains.Annotations;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public class Meta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Count { get; set; }
    }
}