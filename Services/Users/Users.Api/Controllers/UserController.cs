using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Shared.Api.Controllers;
using Shared.Core.Specs;
using Shared.Infrastructure.Data;
using System.Net;
using Users.Core.Entities;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Security;

namespace Users.Api.Controllers
{
    public class UserController : ApiAuthorizeController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IIpConnectionRepository _ipConnectionRepository;
        private readonly IRedisCache _redisCache;
        public UserController(IUserRepository userRepository,
            IIpConnectionRepository ipConnectionRepository,
            IUserService userService,
            IRedisCache redisCache,
            ILogger<UserController> logger,
            Tracer tracer) : base(userService, logger, tracer)
        {
            _userRepository = userRepository;
            _userService = userService;
            _ipConnectionRepository = ipConnectionRepository;
            _redisCache = redisCache;
        }
        #region User
        [HttpGet]
        [Route(nameof(Core.Entities.User))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync([FromQuery] Pagination? pagination, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllUsersAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var users) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<User>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(users);

            users = await _userRepository.GetAllAsync(pagination, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Return users");

            await _redisCache.StoreRedisCacheData(HttpContext, users, cancellationToken);
            return Ok(users);
        }
        [HttpGet]
        [Route(nameof(Core.Entities.User) + "/id")]
        [AllowAnonymous]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User?>> GetUserByIdAsync([FromQuery] int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetUserByIdAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var user) = await _redisCache.GetRedisCacheDataAsync<User>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(user);

            _logger.LogInformation($"User = {requestedUser}. Return user with id = {id}");
            user = await _userRepository.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Return user exists: {user is not null}");

            await _redisCache.StoreRedisCacheData(HttpContext, user, cancellationToken);
            return requestedUser is null ? NotFound() : Ok(user);
        }
        [HttpGet]
        [Route(nameof(Core.Entities.User)+ "/{username}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User?>> GetUserByNameAsync([FromRoute] string username, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetUserByNameAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var user) = await _redisCache.GetRedisCacheDataAsync<User>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(user);

            _logger.LogInformation($"User = {requestedUser}. Return user with name = {username}");
            user = await _userRepository.GetByNameAsync(username, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Return user exists: {user is not null}");

            await _redisCache.StoreRedisCacheData(HttpContext, user, cancellationToken);
            return requestedUser is null ? NotFound() : Ok(requestedUser);
        }
        [HttpGet]
        [Route(nameof(Core.Entities.User) + "/exists/{username}")]
        [AllowAnonymous]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> IsUsersExistsAsync([FromRoute] string username, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetUsersCountAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var exists) = await _redisCache.GetRedisCacheDataAsync<bool>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(exists);

            exists = await _userRepository.IsNameExistsAsync(username, cancellationToken);
            _logger.LogInformation($"User = {username}. User {username} exists = {exists}");

            await _redisCache.StoreRedisCacheData(HttpContext, exists, cancellationToken);
            return Ok(exists);
        }
        [HttpGet]
        [Route(nameof(Core.Entities.User) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetUsersCountAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetUsersCountAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(count);

            count = await _userRepository.GetCountAsync(cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Get users count");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(count);
        }
        [HttpPut]
        [Route(nameof(Core.Entities.User))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<bool>> CreateUserAsync([FromBody] User user, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateUserAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var created = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Create user, id : {user.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, user, cancellationToken);
            return created ? Created(string.Empty, created) : BadRequest();
        }
        [HttpPost]
        [Route(nameof(Core.Entities.User) + "/updatepassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> UpdatePasswordByUserAsync([FromBody] User user, [FromQuery] string oldPassword, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(UpdatePasswordByUserAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            User? userDB;
            using (var spanPart = _tracer.StartActiveSpan(nameof(_userRepository.GetByNameAsync)))
            {
                userDB = await _userRepository.GetByNameAsync(user.Name, cancellationToken);
                if (userDB == null)
                    return StatusCode((int)HttpStatusCode.NotFound);
            }

            _logger.LogInformation($"User = {requestedUser}. Check user password");

            using (var spanPart = _tracer.StartActiveSpan(nameof(_userService.VerifyPassword)))
            {
                var pwdCorrect = await _userService.VerifyPassword(oldPassword, userDB.Password, userDB.Code);
                if (!pwdCorrect)
                    return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            bool userUpdated;
            using (var spanPart = _tracer.StartActiveSpan(nameof(_userRepository.UpdateAsync)))
            {
                userDB.Password = user.Password;
                userUpdated = await _userRepository.UpdateAsync(userDB, cancellationToken);
                _logger.LogInformation($"User = {requestedUser}. Updated user, id : {user.Id}");
            }

            await _redisCache.StoreRedisCacheData(HttpContext, userDB, cancellationToken);
            return Ok(userUpdated);
        }
        [HttpPost]
        [Route(nameof(Core.Entities.User) + "/check")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<bool>> VerifyPasswordByUserAsync([FromBody] User user, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(VerifyPasswordByUserAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            User? userDB;
            using (var spanPart = _tracer.StartActiveSpan(nameof(_userRepository.GetByNameAsync)))
            {
                userDB = await _userRepository.GetByNameAsync(user.Name, cancellationToken);
                if (userDB == null)
                    return StatusCode((int)HttpStatusCode.NotFound);
            }

            using (var spanPart = _tracer.StartActiveSpan(nameof(_userService.VerifyPassword)))
            {
                _logger.LogInformation($"User = {requestedUser}. Check user password");
                var pwdCorrect = await _userService.VerifyPassword(user.Password, userDB.Password, userDB.Code);

                return pwdCorrect ? Ok(true) : Unauthorized();
            }
        }
        #endregion
        #region Ip connection
        [HttpPut]
        [Route(nameof(IpConnection))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<bool>> AddUserIpConnectionAsync([FromQuery] string username, [FromQuery] string ipAddress, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(AddUserIpConnectionAsync));

            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var user) = await _redisCache.GetRedisCacheDataAsync<User>(HttpContext, cancellationToken);
            if (!isCached || user == null)
            { 
                _logger.LogInformation($"User = {requestedUser}. Return user with name = {username}");

                using (var spanPart = _tracer.StartActiveSpan(nameof(_userRepository.GetByNameAsync)))
                { 
                    user = await _userRepository.GetByNameAsync(username, cancellationToken);
                }
                _logger.LogInformation($"User = {requestedUser}. Return user exists: {user is not null}");

                await _redisCache.StoreRedisCacheData(HttpContext, user, cancellationToken);
            }

            using (var spanPart = _tracer.StartActiveSpan(nameof(_ipConnectionRepository.AddAsync)))
            {
                bool created = await _ipConnectionRepository.AddAsync(new IpConnection()
                {
                    IpAddress = ipAddress,
                    UserId = user?.Id,
                    UserName = username,
                });
                return created ? Created(string.Empty, created) : BadRequest();
            }

        }
        #endregion
        #region Dispose

        #endregion
    }
}
