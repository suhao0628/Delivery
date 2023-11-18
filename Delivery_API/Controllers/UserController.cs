using Delivery_API.Models.Dto;
using Delivery_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Mime;

namespace Delivery_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Register 
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] UserRegisterModel register)
        {
            return Ok(await _userService.Register(register));
        }
        #endregion

        #region Log in
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginCredentials login)
        {
            return Ok(await _userService.Login(login));
        }
        #endregion

        #region Log out
        /// <summary>
        /// Log out system user
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response), 500)]
        public IActionResult Logout()
        {
            //to do...

            //var auth = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer"))
            //{
            //    var token = auth.Substring("Bearer".Length).Trim();

            //    _cache.SetStringAsync(token, "1", new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            //    });
            //}
            return Ok(new { message = "Logged Out" });
            

        }
        #endregion

        #region Get user profile
        /// <summary>
        /// Get user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            return Ok(await _userService.GetProfile(userId));
        }
        #endregion

        #region Edit user profile
        /// <summary>
        /// Edit user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> EditProfile([FromBody] UserEditModel profile)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _userService.EditProfile(profile, userId);
            return Ok();
        }
        #endregion
    }
}
