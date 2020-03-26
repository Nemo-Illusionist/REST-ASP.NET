using Radilovsoft.Rest.Infrastructure.Dto;

namespace Radilovsoft.Rest.Infrastructure.Extension
{
    public static class SortDirectionExtension
    {
        public static SortDirection GetOrAsc(this SortDirection? sortDirection)
        {
            return sortDirection ?? SortDirection.Asc;
        }
    }
}