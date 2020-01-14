using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Extension
{
    public static class SortDirectionExtension
    {
        public static SortDirection GetOrAsc(this SortDirection? sortDirection)
        {
            return sortDirection ?? SortDirection.Asc;
        }
    }
}