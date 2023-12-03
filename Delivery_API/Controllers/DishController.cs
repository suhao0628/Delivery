using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Enum;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        #region list of dishes(menu)
        /// <summary>
        /// Get a list of dishes(menu)
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="sorting"></param>
        /// <param name="vegetarian"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishPagedListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDish([FromQuery] DishCategory[] categories, [FromQuery] DishSorting sorting, bool vegetarian, int page = 1)
        {
            return Ok(await _dishService.GetDish(categories, sorting, vegetarian, page));
        }
        #endregion

        #region Get information about concrete dish
        /// <summary>
        /// Get information about concrete dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<DishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDishDetails(Guid id)
        {

            return Ok(await _dishService.GetDishDetails(id));

        }
        #endregion

        #region Checks if user is able to set rating of the dish
        /// <summary>
        /// Checks if user is able to set rating of the dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/rating/check")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckRating(Guid id)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _dishService.CheckRating(id, userId));

        }
        #endregion

        #region Set a rating for a dish
        /// <summary>
        ///  Set a rating for a dish
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ratingScore"></param>
        /// <returns></returns>
        [HttpPost("{id}/rating")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetRating(Guid id, int ratingScore)
        {

            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _dishService.SetRating(id, ratingScore, userId);
            return Ok("Rating set successfully");

        }
        #endregion


    }
}
