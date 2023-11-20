using Delivery_Models.Models.Dto;

namespace Delivery_API.Services.IServices
{
    public interface IBasketService
    {
        Task<IEnumerable<DishBasketDto>> GetBasket(Guid userId);
        Task AddBasket(Guid dishId, Guid userId);
        Task DeleteBasket(Guid dishId, Guid userId, bool increase = false);
    }
}
