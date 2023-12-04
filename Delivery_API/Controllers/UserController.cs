using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore.Storage;

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
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] UserRegisterModel register)
        {
            if (!_userService.IsUniqueUser(register))
            {
                return BadRequest("Username " + register.Email + " is already taken.");
            }
            else
            {
                return Ok(await _userService.Register(register));
            }
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
            return Ok(await _userService.Login(login));
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
        public async Task<ActionResult> Logout()
        {
            var token = Request.Headers["authorization"].Single()?.Split(" ").Last();
            await _userService.Logout(token);
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
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
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
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditProfile([FromBody] UserEditModel profile)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _userService.EditProfile(profile, userId);
            return Ok("Profile updated successfully");
        }
        #endregion
    }
}
