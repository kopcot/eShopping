using Basket.Core.Entities;
using Shared.Infrastructure.Repositories;

namespace Basket.Infrastructure.Repositories
{
    public interface IShoppingCartRepository : IAsyncRepository<ShoppingCart>
    {
        Task<IEnumerable<ShoppingCart>> GetByUserNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
