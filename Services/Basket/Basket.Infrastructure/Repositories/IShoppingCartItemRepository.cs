using Basket.Core.Entities;
using Basket.Core.Specs;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;

namespace Basket.Infrastructure.Repositories
{
    public interface IShoppingCartItemRepository : IAsyncRepository<ShoppingCartItem>
    {
        Task<IEnumerable<ShoppingCartItem>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<ShoppingCartItem>> GetFilteredAsync(ShoppingCartItemSpecParams catalogSpecParam, Pagination? pagination, CancellationToken cancellationToken = default);
        Task<ShoppingCartItem?> GetByIdsAsync(int shoppingCartId, int shoppingCartItemId, CancellationToken cancellationToken = default); 
        Task<long> GetCountAsync(int shoppingCartId, CancellationToken cancellationToken = default);
    }
}
