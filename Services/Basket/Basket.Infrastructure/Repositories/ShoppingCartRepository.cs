using Basket.Core.Entities;
using Basket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;
using Shared.Infrastructure.Extensions;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;


namespace Basket.Infrastructure.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
    {
        protected new readonly BasketContext _dbContext;
        public ShoppingCartRepository(BasketContext basketContext) : base(basketContext)
        {
            _dbContext = basketContext;
        }
        public new async Task<bool> AddAsync(ShoppingCart entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ShoppingCartItems.AttachRange(entity.Items);

            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public new async Task<bool> AddRangeAsync(IEnumerable<ShoppingCart> entities, CancellationToken cancellationToken = default)
        {
            var items = entities.SelectMany(e => e.Items);
            _dbContext.ShoppingCartItems.AttachRange(items);

            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public new async Task<ShoppingCart?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(sc => sc.Items)
                .SingleOrDefaultAsync(sc => sc.Id == id, cancellationToken);
        }
        public async Task<IEnumerable<ShoppingCart>> GetByUserNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(sc => sc.Items)
                .Where(sc => sc.UserName == name)
                .ToListAsync(cancellationToken);
        }
        public new async Task<IEnumerable<ShoppingCart>> GetAllAsync(Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .UsePagination(pagination)
                .Include(sc => sc.Items)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToListAsync(cancellationToken); 
        }
        public new async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var shoppingCart = await _dbSet.Include(sc => sc.Items).FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
            if (shoppingCart == null)
                return false;
            
            _dbContext.RemoveRange(shoppingCart.Items);

            _dbContext.Remove(shoppingCart);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
