using Basket.Core.Entities;
using Basket.Core.Specs;
using Basket.Infrastructure.Data;
using Basket.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Specs;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Repositories;


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
        public async Task<ShoppingCart?> GetByIdAsync(int id, ShoppingCartItemSpecParams? catalogSpecParam = null, CancellationToken cancellationToken = default)
        {
            var shoppingCart = await _dbSet
                .Include(sc => sc.Items.Where(sci => !sci.IsDeleted))
                .SingleOrDefaultAsync(sc => sc.Id == id, cancellationToken);

            if (shoppingCart != null)
                shoppingCart.Items = shoppingCart.Items.AsQueryable().Sort(catalogSpecParam?.Sorting).ToList();

            return shoppingCart;
        }
        public async Task<IEnumerable<ShoppingCart>> GetByUserNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(sc => sc.Items.Where(sci => !sci.IsDeleted))
                .Where(sc => sc.UserName == name)
                .ToArrayAsync(cancellationToken);
        }
        public new async Task<IEnumerable<ShoppingCart>> GetAllAsync(Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Include(sc => sc.Items.Where(sci => !sci.IsDeleted))
                .UsePagination(pagination)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToArrayAsync(cancellationToken);
        }
        public async Task<IEnumerable<ShoppingCart>> GetFilteredAsync(ShoppingCartSpecParams catalogSpecParam, Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Include(sc => sc.Items.Where(sci => !sci.IsDeleted))
                .Filter(catalogSpecParam)
                .Sort(catalogSpecParam?.Sorting)
                .UsePagination(pagination)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToArrayAsync(cancellationToken);
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
