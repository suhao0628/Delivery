
using Delivery_API.Models;
using Delivery_API.Models.Dto;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #region Get information about concrete order
        /// <summary>
        /// Get information about concrete order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            return Ok(await _orderService.GetOrderDetails(id));
        }
        #endregion

        #region Get a list of orders
        /// <summary>
        /// Get a list of orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetOrders()
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _orderService.GetOrders(userId));
        }
        #endregion

        #region Creating the order from dishes in basket
        /// <summary>
        /// Creating the order from dishes in basket
        /// </summary>
        /// <param name="orderCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _orderService.CreateOrder(orderCreateDto, userId);
            return Ok();
        }
        #endregion

        #region Confirm order delivery
        /// <summary>
        /// Confirm order delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> ConfirmDelivery(Guid id)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _orderService.ConfirmDelivery(id, userId);
            return Ok();
        }
        #endregion

    }
}
