using JetBrains.Annotations;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public class PageFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}