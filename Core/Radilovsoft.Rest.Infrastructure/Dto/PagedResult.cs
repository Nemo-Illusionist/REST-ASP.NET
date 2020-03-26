using System.Collections.Generic;
using JetBrains.Annotations;

namespace Radilovsoft.Rest.Infrastructure.Dto
{
    [PublicAPI]
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public Meta Meta { get; set; }
    }
}