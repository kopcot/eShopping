using Basket.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;

namespace Basket.Infrastructure.Data
{
    public class BasketContext : BaseDbContext
    {
        public readonly static Func<BasketContext, int, decimal> TotalPrice = EF.CompileQuery((BasketContext basketContext, int shoppingCartID) =>
            basketContext.ShoppingCarts.First(sc => sc.Id == shoppingCartID).Items.Sum(sc => sc.TotalPrice));
        public readonly static Func<BasketContext, int, Task<decimal>> TotalPriceAsync = EF.CompileAsyncQuery((BasketContext basketContext, int shoppingCartID) =>
            basketContext.ShoppingCarts.First(sc => sc.Id == shoppingCartID).Items.Sum(sc => sc.TotalPrice));
        public static DateTime StartTimeStamp { get; set; } = DateTime.UtcNow;
        public BasketContext(DbContextOptions<BasketContext> options) :base(options)
        {
        }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
