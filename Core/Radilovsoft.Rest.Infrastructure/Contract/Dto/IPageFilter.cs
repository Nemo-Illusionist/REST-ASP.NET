using JetBrains.Annotations;

namespace Radilovsoft.Rest.Infrastructure.Contract.Dto
{
    [PublicAPI]
    public interface IPageFilter
    {
        public int Page { get; }
        public int PageSize { get; }
    }
}