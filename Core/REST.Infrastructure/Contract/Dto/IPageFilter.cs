using JetBrains.Annotations;

namespace REST.Infrastructure.Contract.Dto
{
    [PublicAPI]
    public interface IPageFilter
    {
        public int Page { get; }
        public int PageSize { get; }
    }
}