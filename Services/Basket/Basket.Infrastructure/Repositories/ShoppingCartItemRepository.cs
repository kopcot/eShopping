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
    public class ShoppingCartItemRepository : BaseRepository<ShoppingCartItem>, IShoppingCartItemRepository
    {
        protected new readonly BasketContext _dbContext;
        public ShoppingCartItemRepository(BasketContext basketContext) : base(basketContext)
        {
            _dbContext = basketContext;
        }
        public async Task<IEnumerable<ShoppingCartItem>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(sci => sci.ProductName == name).ToArrayAsync(cancellationToken);
        }
        public async Task<ShoppingCartItem?> GetByIdsAsync(int shoppingCartId, int shoppingCartItemId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ShoppingCarts.Where(sc => sc.Id == shoppingCartId && !sc.IsDeleted).SelectMany(sc => sc.Items).FirstOrDefaultAsync(sci => sci.Id == shoppingCartItemId, cancellationToken);
        }
        public async Task<long> GetCountAsync(int shoppingCartId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ShoppingCarts.Where(sc => sc.Id == shoppingCartId && !sc.IsDeleted).SelectMany(sc => sc.Items).Where(sci => !sci.IsDeleted).AsNoTracking().LongCountAsync(cancellationToken);
        }
        public async Task<IEnumerable<ShoppingCartItem>> GetFilteredAsync(ShoppingCartItemSpecParams catalogSpecParam, Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Filter(catalogSpecParam)
                .Sort(catalogSpecParam?.Sorting)
                .UsePagination(pagination)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToArrayAsync(cancellationToken);
        }
    }
}
