using Delivery_API.Models.Dto;
using Delivery_API.Models.Enum;

namespace Delivery_API.Services.IServices
{
    public interface IDishService
    {
        Task<DishPagedListDto> GetDish(DishCategory[] category, DishSorting sorting, bool vegetarian, int page);
        Task<DishDto> GetDishDetails(Guid id);
        
    }
}
