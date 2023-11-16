using Azure;
using Delivery_API.Models.Dto;
using Delivery_API.Models.Enum;
using Delivery_API.Services.IServices;
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
        [ProducesResponseType(typeof(IEnumerable<DishPagedListDto>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetDish([FromQuery] DishCategory[] categories, [FromQuery] DishSorting sorting,bool vegetarian, int page = 1)
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
        [ProducesResponseType(typeof(IEnumerable<DishDto>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetDishDetails(Guid id)
        {
            return Ok(await _dishService.GetDishDetails(id));
        }
        #endregion
    }
}
