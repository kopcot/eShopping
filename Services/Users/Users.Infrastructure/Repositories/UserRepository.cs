using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;
using Users.Core.Entities;
using Users.Infrastructure.Data;
using Users.Infrastructure.Security;

namespace Users.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        protected new readonly UserContext _dbContext;
        protected readonly IUserService _userService;
        public UserRepository(UserContext userContext, IUserService userService) : base(userContext)
        {
            _dbContext = userContext;
            _userService = userService;
        }
        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower(), cancellationToken);
        }
        public async new Task<bool> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _userService.RefreshCode(user);
            user.Password = await _userService.HashPasswordAsync(user.Password, user.Code);

            await _dbSet.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async new Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            await _userService.RefreshCode(user);
            user.Password = await _userService.HashPasswordAsync(user.Password, user.Code);

            _dbSet.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}
