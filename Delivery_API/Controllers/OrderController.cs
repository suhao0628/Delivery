using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _orderService.CreateOrder(orderCreateDto, userId);
            return Ok("Order created successfully");
        }
        #endregion

        #region Confirm order delivery
        /// <summary>
        /// Confirm order delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/status")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmDelivery(Guid id)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _orderService.ConfirmDelivery(id, userId);
            return Ok("Confirm Delivered");
        }
        #endregion

    }
}
