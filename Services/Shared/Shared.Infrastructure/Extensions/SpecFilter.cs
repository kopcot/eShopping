using Shared.Core.Entities;
using Shared.Core.Specs;

namespace Shared.Infrastructure.Extensions
{
    public static class SpecFilter
    {
        public static IQueryable<T> UsePagination<T>(this IQueryable<T> entity, Pagination? pagination) where T : BaseEntity
        {
            if (pagination is not null && pagination.UsePagination)
            {
                entity = entity
                    .Skip(pagination.PageIndex * pagination.PageSize)
                    .Take(pagination.PageSize);
            }
            return entity;
        }
    }
}
