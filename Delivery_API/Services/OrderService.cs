using Delivery_API.Data;
using Delivery_API.Exceptions;
using Delivery_API.Services.IServices;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Delivery_Models.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace Delivery_API.Services
{
    public class OrderService: IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> GetOrderDetails(Guid orderId)
        {
            var carts = await _context.OrderBaskets.Where(w => w.OrderId == orderId).Include(w => w.Order).ToListAsync();
            if (!carts.Any())
            {
                throw new Exception();
            }


            List<DishBasketDto> dishBasketDtos = new();
            foreach (var item in carts)
            {
                DishBasketDto dishBasketDto = new()
                {
                    Id = item.DishId,
                    Name = item.Name,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    Amount = item.Amount,
                    Image = item.Image
                };
                dishBasketDtos.Add(dishBasketDto);
            }
            OrderDto orderDto = new()
            {
                Id = carts.FirstOrDefault().OrderId,
                DeliveryTime = carts.FirstOrDefault().Order.DeliveryTime,
                OrderTime = carts.FirstOrDefault().Order.OrderTime,
                Status = carts.FirstOrDefault().Order.Status,
                Price = carts.FirstOrDefault().Price,
                Dishes = dishBasketDtos,
                Address = carts.FirstOrDefault().Order.Address
            };

 
            return orderDto;
        }


        public async Task<List<OrderInfoDto>> GetOrders(Guid userId)
        {
            List<OrderInfoDto> orderInfoDtos = new();
            var orders = await _context.Orders.Where(w => w.UserId == userId).ToListAsync();
            foreach (var item in orders)
            {
                OrderInfoDto orderInfoDto = new()
                {
                    Id = item.Id,
                    DeliveryTime = item.DeliveryTime,
                    OrderTime = item.OrderTime,
                    Price = item.Price,
                    Status = item.Status
                };
                orderInfoDtos.Add(orderInfoDto);
            }

            return orderInfoDtos;
        }

        public async Task CreateOrder(OrderCreateDto orderCreateDto, Guid userId)
        {
            var baskets = await _context.Baskets.Where(b => b.UserId == userId).Include(b => b.Dish).ToListAsync();
            if (baskets.Any())
            {
                Order order = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Address = orderCreateDto.Address,
                    DeliveryTime = orderCreateDto.DeliveryTime
                };

                int allTotalPrice = 0;

                List<OrderBasket> carts = new();

                foreach (var basketItem in baskets)
                {
                    allTotalPrice += basketItem.Dish.Price * basketItem.Amount;

                    //Migration data from basket to cart
                    OrderBasket cart = new()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        DishId = basketItem.Dish.Id,
                        Name = basketItem.Dish.Name,
                        Price = basketItem.Dish.Price,
                        TotalPrice = basketItem.Dish.Price * basketItem.Amount,
                        Amount = basketItem.Amount,
                        Image = basketItem.Dish.Image
                    };
                    carts.Add(cart);
                }

                order.Price = allTotalPrice;
                order.OrderTime = DateTime.Now;
                order.Status = OrderStatus.InProcess;

                await _context.AddRangeAsync(carts);
                await _context.AddAsync(order);
                //Delete basket
                _context.RemoveRange(baskets);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception();
            }

        }

        public async Task ConfirmDelivery(Guid orderId, Guid userId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            if (order.UserId != userId)
            {
                throw new NotFoundException("Invalid order user");
            }

            order.Status = OrderStatus.Delivered;
            _context.Update(order);
            await _context.SaveChangesAsync();
        }


    }
}
