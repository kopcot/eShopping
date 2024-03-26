using Basket.Core.Entities;
using Basket.Infrastructure.Data;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Basket.InfrastructureTests
{
    public class BasketContextTests : IDisposable
    {
        private readonly BasketContext _context;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IUserService _userService;
        public BasketContextTests()
        {
            IUserService userService = new UserService();

            var options = new DbContextOptionsBuilder<BasketContext>()
                .UseMySQL("Server=192.168.1.200;Port=3307;Database=EShopping;User=user;Password=E1Z/0UbU7*FZq/A.;")
                .Options;
            _context = new BasketContext(options);
            //_context.Database.EnsureDeleted();
            _context.Database.Migrate();

            _shoppingCartRepository = new ShoppingCartRepository(_context);
            _shoppingCartItemRepository = new ShoppingCartItemRepository(_context);
            _userService = new UserService();
        }
        public void Dispose()
        {
            //_context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Fact]
        public async Task StartTest_ReturnAllwaysTrue()
        {
            await Task.CompletedTask;

            Assert.True(true);
        }
        [Fact]
        public async Task CanConnect_ReturnTrue()
        {
            bool canConect = await _shoppingCartRepository.CanConnectAsync();
            canConect &= await _shoppingCartItemRepository.CanConnectAsync();

            Assert.True(canConect);
        }
        //[Fact]
        //public async Task AddContext_ReturnTrue()
        //{
        //    List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
        //    var count = (uint)new Random().Next(5, 500000);
        //    for (int j = 0; j < count; j++)
        //    {
        //        ShoppingCart shoppingCart = new ShoppingCart() 
        //        { 
        //            UserName = RandomString(10), 
        //            Items = new List<ShoppingCartItem>() 
        //        };
        //        var itemCount = (uint)new Random().Next(5, 50);
        //        for (int i = 0; i < itemCount; i++) 
        //        {
        //            ShoppingCartItem shoppingCartItem = new ShoppingCartItem()
        //            {
        //                ProductName = "ProductName" + i.ToString(),
        //                Quantity = new Random().Next(0, 50),
        //                Price = (decimal)new Random().NextDouble() * 100,
        //                ImageFile = null
        //            };
        //            shoppingCart.Items.Add(shoppingCartItem);
        //        }
        //        shoppingCarts.Add(shoppingCart);
        //
        //        //await _shoppingCartRepository.AddAsync(shoppingCart);
        //        if (j % 10000 == 0) 
        //        {
        //            await _shoppingCartRepository.AddRangeAsync(shoppingCarts);
        //            shoppingCarts.Clear();
        //        }
        //    }
        //    
        //
        //    Assert.True(true);
        //}
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
