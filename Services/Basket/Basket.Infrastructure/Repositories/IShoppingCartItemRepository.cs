using Basket.Core.Entities;
using Shared.Infrastructure.Repositories;

namespace Basket.Infrastructure.Repositories
{
    public interface IShoppingCartItemRepository : IAsyncRepository<ShoppingCartItem>
    {
        Task<IEnumerable<ShoppingCartItem>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem?> GetByIdsAsync(int shoppingCartId, int shoppingCartItemId, CancellationToken cancellationToken = default);
    }
}
