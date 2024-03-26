using Basket.Core.Entities;
using System.Net;

namespace eShopping.Client.Data
{
    public interface IShoppingCartService
    {
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetShoppingCartsAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : ShoppingCart;
        Task<(bool, HttpStatusCode?, Exception?, long?)> GetShoppingCartsCountAsync<T>(CancellationToken? cancellationToken = null) where T : ShoppingCart;
        Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken = null) where T : ShoppingCart;
        Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken = null) where T : ShoppingCartItem;
        Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdsAsync<T>(int shoppingCartId, int shoppingCartItemId, CancellationToken? cancellationToken = null) where T : ShoppingCartItem;

    }
}
