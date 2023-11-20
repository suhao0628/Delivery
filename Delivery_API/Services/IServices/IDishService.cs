using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Enum;

namespace Delivery_API.Services.IServices
{
    public interface IDishService
    {
        Task<DishPagedListDto> GetDish(DishCategory[] category, DishSorting sorting, bool vegetarian, int page);
        Task<DishDto> GetDishDetails(Guid id);

        Task<bool> CheckRating(Guid id, Guid userId);
        Task SetRating(Guid id, int ratingScore, Guid userId);
    }
}
