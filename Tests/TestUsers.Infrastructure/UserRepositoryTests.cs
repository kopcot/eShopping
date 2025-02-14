using Moq;
using Users.Core.Entities;
using Users.Infrastructure.Data;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Security;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Users.InfrastructureTests
{
    /// <summary>
    /// ChatGPT generated
    /// </summary>
    public class UserRepositoryTests
    {
        //private readonly Mock<UserContext> _mockDbContext;
        private readonly Mock<UserService> _mockUserService;
        private readonly UserRepository _userRepository;
        private readonly Mock<DbSet<User>> _mockDbSet;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseMySQL("Server=192.168.1.200;Port=3307;Database=EShopping;User=user;Password=E1Z/0UbU7*FZq/A.;")
                .Options;

            _mockUserService = new Mock<UserService>();
            _mockDbSet = new Mock<DbSet<User>>();

            _userRepository = new UserRepository(new UserContext(options), (IUserService)_mockUserService.Object);
        }
        [Fact]
        public async Task CanConnectAsync_ReturnsTrue_WhenCanConnect()
        {
            // Arrange

            // Act
            var result = await _userRepository.CanConnectAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userName = "test";
            var users = new List<User>
        {
            new User { Name = userName }
        }.AsQueryable();

            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var result = await _userRepository.GetByNameAsync(userName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userName, result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userName = "nonexistentUser";
            var users = new List<User>().AsQueryable();

            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var result = await _userRepository.GetByNameAsync(userName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsNameExistsAsync_ReturnsTrue_WhenUserNameExists()
        {
            // Arrange
            var userName = "test";
            var users = new List<User>
        {
            new User { Name = userName }
        }.AsQueryable();

            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var result = await _userRepository.IsNameExistsAsync(userName);
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsNameExistsAsync_ReturnsFalse_WhenUserNameDoesNotExist()
        {
            // Arrange
            var userName = "nonexistentUser";
            var users = new List<User>().AsQueryable();

            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Act
            var result = await _userRepository.IsNameExistsAsync(userName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_AddsUserToDatabase()
        {
            // Arrange
            var user = new User { Name = "test", Password = "password" };
            _mockUserService.Setup(s => s.RefreshCode(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUserService.Setup(s => s.HashPasswordAsync(It.IsAny<string>(), It.IsAny<uint>())).ReturnsAsync("hashedPassword");

            // Act
            var result = await _userRepository.AddAsync(user);

            // Assert
            _mockDbSet.Verify(m => m.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once());
            //_mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.True(result);
        }

        // Additional tests for UpdateAsync, GetByIdAsync, etc.
    }
}
