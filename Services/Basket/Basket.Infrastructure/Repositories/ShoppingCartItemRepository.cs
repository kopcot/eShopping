using Basket.Core.Entities;
using Basket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using System.Threading;


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
            return await _dbSet.Where(sci => sci.ProductName == name).ToListAsync(cancellationToken);
        }
        public async Task<ShoppingCartItem?> GetByIdsAsync(int shoppingCartId, int shoppingCartItemId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ShoppingCarts.Where(sc => sc.Id == shoppingCartId).SelectMany(sc => sc.Items).FirstOrDefaultAsync(sci => sci.Id ==  shoppingCartItemId, cancellationToken);
        }

    }
}
