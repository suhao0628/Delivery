using Delivery_API.Models;
using Delivery_API.Models.Dto;
using Delivery_API.Models.Entity;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Delivery_API.Controllers
{
    [Route("api/basket")]
    [ApiController]
    [Authorize]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IEnumerable<DishBasketDto>), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetBasket()
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            //Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(await _basketService.GetBasket(userId));
        }
        #endregion

        #region Add dish to cart
        /// <summary>
        /// Add dish to cart
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        [HttpPost("dish/{dishId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> AddBasket(Guid dishId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _basketService.AddBasket(dishId, userId);
            return Ok();
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> DeleteBasket(Guid dishId, bool increase)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _basketService.DeleteBasket(dishId, userId, increase);
            return Ok();
        }
        #endregion

    }
}
