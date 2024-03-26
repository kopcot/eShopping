using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IRedisCache _redisCache;
        public UserController(IUserRepository userRepository,
            IRedisCache redisCache,
            IUserService userService,
            ILogger<UserController> logger) : base(userService, logger)
        {
            _userRepository = userRepository;
            _userService = userService;
            _redisCache = redisCache;
        }
        #region User
        [HttpGet]
        [Route(nameof(Core.Entities.User))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync([FromQuery] Pagination? pagination, CancellationToken cancellationToken = default)
        {

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
        [Route(nameof(Core.Entities.User) + "/{id}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User?>> GetUserByIdAsync([FromRoute] int id, CancellationToken cancellationToken = default)
        {
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
            return requestedUser is null ? NotFound() : Ok(requestedUser);
        }
        [HttpGet]
        [Route(nameof(Core.Entities.User) + "/{username}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User?>> GetUserByNameAsync([FromRoute] string username, CancellationToken cancellationToken = default)
        {
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
        [Route(nameof(Core.Entities.User) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetUsersCountAsync(CancellationToken cancellationToken = default)
        {
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
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<User>> CreateUserAsync([FromBody] User user, CancellationToken cancellationToken = default)
        {
            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Create user, id : {user.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, user, cancellationToken);
            return Created(string.Empty, user);
        }
        [HttpPost]
        [Route(nameof(Core.Entities.User) + "/updatepassword")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<User>> UpdatePasswordByUserAsync([FromBody] User user, [FromQuery] string oldPassword, CancellationToken cancellationToken = default)
        {
            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var userDB = await _userRepository.GetByNameAsync(user.Name, cancellationToken);
            if (userDB == null)
                return StatusCode((int)HttpStatusCode.NotFound);

            _logger.LogInformation($"User = {requestedUser}. Check user password");
            var pwdCorrect = await _userService.VerifyPassword(oldPassword, userDB.Password, userDB.Code);
            if (!pwdCorrect)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            await _userRepository.UpdateAsync(userDB, cancellationToken);
            _logger.LogInformation($"User = {requestedUser}. Updated user, id : {user.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, userDB, cancellationToken);
            return Ok(userDB);
        }
        [HttpPost]
        [Route(nameof(Core.Entities.User) + "/check")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> VerifyPasswordByUserAsync([FromBody] User user, CancellationToken cancellationToken = default)
        {
            (var result, var requestedUser, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var userDB = await _userRepository.GetByNameAsync(user.Name, cancellationToken);
            if (userDB == null)
                return StatusCode((int)HttpStatusCode.NotFound);

            _logger.LogInformation($"User = {requestedUser}. Check user password");
            var pwdCorrect = await _userService.VerifyPassword(user.Password, userDB.Password, userDB.Code);

            return pwdCorrect ? Ok(true) : Unauthorized();
        }
        #endregion
        #region Dispose

        #endregion
    }
}
