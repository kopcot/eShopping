using Shared.Core.Entities;
using Shared.Core.Specs;
using System.Threading;

namespace Shared.Infrastructure.Repositories
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Pagination? pagination = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllWithSoftDeletedAsync(Pagination? pagination = null, CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(CancellationToken cancellationToken = default);
        Task<long> GetCountWithSoftDeletedAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> HardDeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
