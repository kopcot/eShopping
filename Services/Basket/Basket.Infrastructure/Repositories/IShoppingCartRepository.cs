using Basket.Core.Entities;
using Basket.Core.Specs;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;

namespace Basket.Infrastructure.Repositories
{
    public interface IShoppingCartRepository : IAsyncRepository<ShoppingCart>
    {
        Task<ShoppingCart?> GetByIdAsync(int id, ShoppingCartItemSpecParams? catalogSpecParam = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<ShoppingCart>> GetByUserNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<ShoppingCart>> GetFilteredAsync(ShoppingCartSpecParams catalogSpecParam, Pagination? pagination, CancellationToken cancellationToken = default);
    }
}
