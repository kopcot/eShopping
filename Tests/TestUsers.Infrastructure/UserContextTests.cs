using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Data;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Security;

namespace Users.InfrastructureTests
{
    public class UserContextTests : IDisposable
    {
        private readonly UserContext _context;
        private readonly IUserRepository _repository;
        public UserContextTests()
        {
            IUserService userService = new UserService();

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseMySQL("Server=192.168.1.200;Port=3307;Database=EShopping;User=user;Password=E1Z/0UbU7*FZq/A.;")
                .Options;
            _context = new UserContext(options);
            //_context.Database.EnsureDeleted();
            _context.Database.Migrate();

            _repository = new UserRepository(_context, userService);
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
            bool canConect = await _repository.CanConnectAsync();

            Assert.True(canConect);
        }
    }
}
