using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using Shared.Core.Specs;
using Shared.Infrastructure.Extensions;
using System.Threading;

namespace Shared.Infrastructure.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _dbContext;

        protected DbSet<T> _dbSet => _dbContext.Set<T>();
        public BaseRepository(DbContext dDContext)
        {
            _dbContext = dDContext;
        }
        public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        { 
            return await _dbContext.Database.CanConnectAsync(cancellationToken);
        }
        public async Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .UsePagination(pagination)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        }
        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                //.Select(e => e.Id)
                .LongCountAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(id, cancellationToken);
            if (entity == null) 
                return false;

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
