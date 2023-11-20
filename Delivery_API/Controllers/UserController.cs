using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;

namespace Delivery_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDistributedCache _cache;
        private readonly IOptions<JwtConfigurations> _jwtOptions;

        public UserController(IUserService userService, IDistributedCache cache, IOptions<JwtConfigurations> jwtOptions)
        {
            _userService = userService;
            _cache = cache;
            _jwtOptions = jwtOptions;
        }

        #region Register 
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginCredentials login)
        {
            try
            {
                return Ok(await _userService.Login(login));
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion

        #region Log out
        /// <summary>
        /// Log out system user
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public IActionResult Logout()
        {
            try
            {
                string token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

                var expirationMinutes = _jwtOptions.Value.Expires;
                if (!token.IsNullOrEmpty())
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expirationMinutes)
                    };
                    _cache.SetString(token, "", options);

                    return Ok(new { message = "Logged Out" });
                }
                return BadRequest();
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }


        }
        #endregion

        #region Get user profile
        /// <summary>
        /// Get user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
                return Ok(await _userService.GetProfile(userId));
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion

        #region Edit user profile
        /// <summary>
        /// Edit user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditProfile([FromBody] UserEditModel profile)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
                await _userService.EditProfile(profile, userId);
                return Ok();
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion
    }
}
