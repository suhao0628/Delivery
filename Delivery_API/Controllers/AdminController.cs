using Delivery_API.Services.IServices;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : ControllerBase
    {
        private readonly IDishService _dishService;

        public AdminController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Dish>> CreateDish(DishDto dishDto)
        {
            return Ok(await _dishService.CreateDish(dishDto));
        }

        [HttpPut("update/{dishId}")]
        public async Task<ActionResult> UpdateDish(Guid dishId, DishDto dishDto)
        {
            await _dishService.UpdateDish(dishId, dishDto);
            return Ok("Dish Updated successfully");
        }

        [HttpDelete("delete/{dishId}")]
        public async Task<ActionResult> DeleteDish(Guid dishId)
        {
            await _dishService.DeleteDish(dishId);
            return Ok("Dish Deleted successfully");
        }
    }
}
