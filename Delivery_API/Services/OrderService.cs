using AutoMapper;
using Delivery_API.Data;
using Delivery_API.Models.Dto;
using Delivery_API.Models.Entity;
using Delivery_API.Models.Enum;
using Delivery_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Delivery_API.Services
{
    public class OrderService: IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetOrderDetails(Guid orderId)
        {
            var carts = await _context.OrderBaskets.Where(w => w.OrderId == orderId).Include(w => w.Order).ToListAsync();
            if (!carts.Any())
            {
                throw new Exception();
            }


            List<DishBasketDto> dishBasketDtos = new List<DishBasketDto>();
            foreach (var item in carts)
            {
                DishBasketDto dishBasketDto = new DishBasketDto();
                dishBasketDto.Id = item.DishId;
                dishBasketDto.Name = item.Name;
                dishBasketDto.Price = item.Price;
                dishBasketDto.TotalPrice = item.TotalPrice;
                dishBasketDto.Amount = item.Amount;
                dishBasketDto.Image = item.Image;
                dishBasketDtos.Add(dishBasketDto);
            }
            OrderDto orderDto = new()
            {
                Id = carts.FirstOrDefault().OrderId,
                DeliveryTime = carts.FirstOrDefault().Order.DeliveryTime,
                OrderTime = carts.FirstOrDefault().Order.OrderTime,
                Status = carts.FirstOrDefault().Order.Status,
                Price = carts.FirstOrDefault().Price,
                Dishes = dishBasketDtos,///_mapper.Map<List<DishBasketDto>>(carts),
                Address = carts.FirstOrDefault().Order.Address
            };

 
            return orderDto;
        }


        public async Task<List<OrderInfoDto>> GetOrders(Guid userId)
        {
            List<OrderInfoDto> orderInfoDtos = new List<OrderInfoDto>();
            var orders = await _context.Orders.Where(w => w.UserId == userId).ToListAsync();
            foreach (var item in orders)
            {
                OrderInfoDto orderInfoDto = new OrderInfoDto();
                orderInfoDto.Id = item.Id;
                orderInfoDto.DeliveryTime = item.DeliveryTime;
                orderInfoDto.OrderTime = item.OrderTime;
                orderInfoDto.Price = item.Price;
                orderInfoDto.Status = item.Status;
                orderInfoDtos.Add(orderInfoDto);
            }

            return orderInfoDtos;//_mapper.Map<List<OrderInfoDto>>(orders);
        }

        public async Task CreateOrder(OrderCreateDto orderCreateDto, Guid userId)
        {
            var baskets = await _context.Baskets.Where(b => b.UserId == userId).Include(b => b.Dish).ToListAsync();
            if (baskets.Any())
            {
                //var order = _mapper.Map<Order>(orderCreateDto);
                Order order = new Order();
                order.Id = Guid.NewGuid();
                order.UserId = userId;
                order.Address = orderCreateDto.Address;
                order.DeliveryTime = orderCreateDto.DeliveryTime;

                int allTotalPrice = 0;

                List<OrderBasket> carts = new List<OrderBasket>();

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
                throw new Exception();
            }

            if (order.UserId != userId)
            {
                throw new Exception();
            }

            order.Status = OrderStatus.Delivered;
            _context.Update(order);
            await _context.SaveChangesAsync();
        }


    }
}
