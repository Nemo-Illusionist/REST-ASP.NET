using System;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using REST.Infrastructure.Dto;

namespace REST.Infrastructure.Extension
{
    public static class QueryableExtension
    {
        public static IQueryable<T> FilterPage<T>(this IQueryable<T> source, PageFilter pageFilter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pageFilter == null) throw new ArgumentNullException(nameof(pageFilter));

            return source.FilterPage(pageFilter.Page, pageFilter.PageSize);
        }

        public static IQueryable<T> FilterPage<T>(this IQueryable<T> source, int page, int pageSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ProjectTo<T>([NotNull] this IQueryable source, [NotNull] IMapper mapper)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return mapper.ProjectTo<T>(source);
        }
    }
}