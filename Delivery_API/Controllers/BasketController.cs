using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Controllers
{
    [Route("api/basket")]
    [ApiController]
    [Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        #region Get user cart
        /// <summary>
        /// Get user cart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishBasketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBasket()
        {
            try
            {
                var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
                return Ok(await _basketService.GetBasket(userId));
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion

        #region Add dish to cart
        /// <summary>
        /// Add dish to cart
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        [HttpPost("dish/{dishId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBasket(Guid dishId)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

                await _basketService.AddBasket(dishId, userId);
                return Ok("Dish added to basket successfully");
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion

        #region Descrease the number of dishes in the cart
        /// <summary>
        /// Descrease the number of dishes in the cart (if increase=true),or remove the dish completely(increase=false)
        /// </summary>
        /// <param name="dishId"></param>
        /// <param name="increase"></param>
        /// <returns></returns>
        /// 
        [HttpDelete("dish/{dishId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBasket(Guid dishId, bool increase)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
                await _basketService.DeleteBasket(dishId, userId, increase);
                return Ok("Dish removed from basket successfully");
            }
            catch
            {
                return StatusCode(500, new Response { Status = "Error", Message = "Error in request" });
            }
        }
        #endregion

    }
}
