using Shared.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Core.Entities;
using Users.Infrastructure.Data;
using Users.Infrastructure.Security;

namespace Users.Infrastructure.Repositories
{
    public class IpConnectionRepository : BaseRepository<IpConnection>, IIpConnectionRepository
    {
        protected new readonly UserContext _dbContext;
        protected readonly IUserService _userService;
        public IpConnectionRepository(UserContext userContext, IUserService userService) : base(userContext)
        {
            _dbContext = userContext;
            _userService = userService;
        }
    }
}
