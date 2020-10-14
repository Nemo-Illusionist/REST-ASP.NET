using System.Collections.Generic;

namespace Radilovsoft.Rest.Infrastructure.Dto
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public Meta Meta { get; set; }
    }
}