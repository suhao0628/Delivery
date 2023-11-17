using Delivery_API.Models.Dto;

namespace Delivery_API.Services.IServices
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderDetails(Guid orderId);
        Task<List<OrderInfoDto>> GetOrders(Guid userId);
        Task CreateOrder(OrderCreateDto orderCreateDto, Guid userId);
        Task ConfirmDelivery(Guid orderId, Guid userId);
    }
}
